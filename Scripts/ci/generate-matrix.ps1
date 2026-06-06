# Dynamically generates a build matrix based on changed files, projects.json and dependencies.
[CmdletBinding()]
param(
    [string]$EventName = $env:EVENT_NAME,
    [string]$FilterOutputs = $env:FILTER_OUTPUTS,
    [string]$GithubOutput = $env:GITHUB_OUTPUT
)

$ErrorActionPreference = "Stop"

$ProjectsJsonPath = Join-Path $PSScriptRoot "projects.json"
if (-not (Test-Path $ProjectsJsonPath)) {
    Write-Error "Error: Projects configuration file not found at $ProjectsJsonPath"
    exit 1
}

# Parse projects configuration
$Projects = Get-Content -Raw -Path $ProjectsJsonPath | ConvertFrom-Json

# Parse filter outputs safely without Core-only -AsHashtable
$Filters = $null
if (-not [string]::IsNullOrEmpty($FilterOutputs)) {
    try {
        $Filters = $FilterOutputs | ConvertFrom-Json
    } catch {
        # ignore parse errors, keep null
    }
}

# Determine which projects are directly active
$DirectActive = [System.Collections.Generic.List[string]]::new()
foreach ($name in $Projects.PSObject.Properties.Name) {
    $proj = $Projects.$name
    $filterKey = $proj.filter_key
    if ([string]::IsNullOrEmpty($filterKey)) {
        $filterKey = $name
    }
    
    $isActive = $false
    if ($EventName -eq "workflow_dispatch") {
        $isActive = $true
    } elseif ($Filters -and $Filters.PSObject.Properties[$filterKey]) {
        $val = $Filters.$filterKey
        if ($val -eq $true -or $val -eq "true") {
            $isActive = $true
        }
    }
    
    if ($isActive) {
        $DirectActive.Add($name) | Out-Null
    }
}

# Compute transitive closure of active projects
$ActiveProjects = [System.Collections.Generic.HashSet[string]]::new()

function Activate-Project {
    param([string]$name)
    if ($ActiveProjects.Contains($name)) {
        return
    }
    $ActiveProjects.Add($name) | Out-Null
    
    # Find all other projects that depend on this one
    foreach ($otherName in $Projects.PSObject.Properties.Name) {
        $otherProj = $Projects.$otherName
        if ($otherProj.dependencies -and $otherProj.dependencies -contains $name) {
            Activate-Project $otherName
        }
    }
}

# Activate all changed projects
foreach ($name in $DirectActive) {
    Activate-Project $name
}

# Build the final matrix list
$MatrixItems = [System.Collections.Generic.List[PSCustomObject]]::new()
foreach ($name in $Projects.PSObject.Properties.Name) {
    if ($ActiveProjects.Contains($name)) {
        $proj = $Projects.$name
        $MatrixItems.Add([PSCustomObject]@{
            name      = $name
            path      = $proj.path
            test_path = $proj.test_path
        })
    }
}

$MatrixJson = $MatrixItems | ConvertTo-Json -Compress

Write-Host "Generated matrix: $MatrixJson"

# Output results to GitHub Actions or console
if (-not [string]::IsNullOrEmpty($GithubOutput)) {
    if ($MatrixItems.Count -eq 0) {
        Add-Content -Path $GithubOutput -Value "has_projects=false"
        Add-Content -Path $GithubOutput -Value "matrix={\`"include\`":[]}"
    } else {
        Add-Content -Path $GithubOutput -Value "has_projects=true"
        Add-Content -Path $GithubOutput -Value "matrix<<EOF"
        Add-Content -Path $GithubOutput -Value "{\`"include\`":$MatrixJson}"
        Add-Content -Path $GithubOutput -Value "EOF"
    }
} else {
    if ($MatrixItems.Count -eq 0) {
        Write-Host "has_projects=false"
        Write-Host "matrix={\`"include\`":[]}"
    } else {
        Write-Host "has_projects=true"
        Write-Host "matrix={\`"include\`":$MatrixJson}"
    }
}
