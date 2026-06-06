<#
.SYNOPSIS
    Downloads and updates the Dokfx template to the latest released version from GitHub.
.DESCRIPTION
    This script queries the GitHub API for the latest release of KryKomDev/Dokfx,
    downloads the 'dokfx-template.zip' asset (or falls back to the source zipball),
    clears the existing files in Docs/templates/dokfx, and extracts the new ones.
.EXAMPLE
    .\update-dokfx.ps1
#>

$ErrorActionPreference = "Stop"

# Repository details
$Repo = "KryKomDev/Dokfx"
$ApiUrl = "https://api.github.com/repos/$Repo/releases/latest"

# Target template directory relative to the script location
$TargetDir = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "../Docs/templates/dokfx"))

# Force TLS 1.2
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Write-Host "Fetching latest release information from GitHub API..." -ForegroundColor Cyan
try {
    $Release = Invoke-RestMethod -Uri $ApiUrl -Headers @{ "User-Agent" = "PowerShell" }
} catch {
    Write-Error "Failed to fetch release info from GitHub API: $_"
    exit 1
}

$TagName = $Release.tag_name
Write-Host "Latest release tag: $TagName" -ForegroundColor Green

# Find the dokfx-template.zip asset
$Asset = $Release.assets | Where-Object { $_.name -eq "dokfx-template.zip" } | Select-Object -First 1

if ($Asset) {
    $DownloadUrl = $Asset.browser_download_url
    Write-Host "Found release asset: $($Asset.name) ($($Asset.size) bytes)" -ForegroundColor Gray
} else {
    Write-Warning "Could not find 'dokfx-template.zip' in release assets. Falling back to source code zipball..."
    $DownloadUrl = $Release.zipball_url
}

# Download to a temporary location
$TempZip = Join-Path ([System.IO.Path]::GetTempPath()) ("dokfx-template-" + [Guid]::NewGuid().ToString() + ".zip")
Write-Host "Downloading template from: $DownloadUrl..." -ForegroundColor Cyan

try {
    Invoke-WebRequest -Uri $DownloadUrl -OutFile $TempZip
} catch {
    Write-Error "Failed to download template: $_"
    exit 1
}

# Ensure destination directory exists and is empty
if (-not (Test-Path $TargetDir)) {
    Write-Host "Creating target directory: $TargetDir" -ForegroundColor Cyan
    New-Item -ItemType Directory -Path $TargetDir -Force | Out-Null
} else {
    Write-Host "Clearing existing template files in: $TargetDir" -ForegroundColor Cyan
    Remove-Item -Path "$TargetDir\*" -Recurse -Force
}

# Temporary extraction directory
$TempExtractDir = Join-Path ([System.IO.Path]::GetTempPath()) ([Guid]::NewGuid().ToString())
New-Item -ItemType Directory -Path $TempExtractDir -Force | Out-Null

try {
    Write-Host "Extracting template archive..." -ForegroundColor Cyan
    Expand-Archive -Path $TempZip -DestinationPath $TempExtractDir -Force
    
    # Handle wrapper folder structure (if any, like in source zipballs)
    $RootItems = Get-ChildItem -Path $TempExtractDir
    $SourceDir = $TempExtractDir
    
    # If the zip has a single root wrapper directory, use that as source
    if ($RootItems.Count -eq 1 -and $RootItems[0].PSIsContainer -and $RootItems[0].Name -notIn "partials", "public") {
        $SourceDir = $RootItems[0].FullName
        Write-Host "Found nested directory structure inside zip: $($RootItems[0].Name)" -ForegroundColor Gray
    }
    
    # Copy extracted template files to the target templates directory
    Write-Host "Copying template files to target location..." -ForegroundColor Cyan
    Get-ChildItem -Path "$SourceDir\*" | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $TargetDir -Recurse -Force
    }
    
    Write-Host "Template updated successfully to version $TagName!" -ForegroundColor Green
} catch {
    Write-Error "Failed to extract or copy template files: $_"
    exit 1
} finally {
    # Clean up temp files
    if (Test-Path $TempZip) {
        Remove-Item -Path $TempZip -Force
    }
    if (Test-Path $TempExtractDir) {
        Remove-Item -Path $TempExtractDir -Recurse -Force
    }
}
