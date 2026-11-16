<#
.SYNOPSIS
    Replaces a specific 1-based line range in a file with new content.
.DESCRIPTION
    Safely replaces lines [StartLine..EndLine] in a target file, preserving UTF-8 encoding
    and original newline formatting. Useful when exact string matching fails due to subtle
    whitespace or line-ending mismatches.
.PARAMETER Path
    Path to the file to modify.
.PARAMETER StartLine
    1-based start line number to replace (inclusive).
.PARAMETER EndLine
    1-based end line number to replace (inclusive).
.PARAMETER Content
    The replacement string.
.PARAMETER ContentFile
    Optional path to a file containing the replacement text.
#>
param(
    [Parameter(Mandatory=$true)][string]$Path,
    [Parameter(Mandatory=$true)][int]$StartLine,
    [Parameter(Mandatory=$true)][int]$EndLine,
    [Parameter(Mandatory=$false)][string]$Content,
    [Parameter(Mandatory=$false)][string]$ContentFile
)

if (-not (Test-Path $Path)) {
    Write-Error "File not found: $Path"
    exit 1
}

if ($ContentFile -and (Test-Path $ContentFile)) {
    $replacement = [System.IO.File]::ReadAllLines($ContentFile)
} elseif ($PSBoundParameters.ContainsKey('Content')) {
    $replacement = $Content -split "\r?\n"
} else {
    Write-Error "Either -Content or -ContentFile must be provided."
    exit 1
}

$lines = [System.IO.File]::ReadAllLines((Resolve-Path $Path).Path)
$totalLines = $lines.Length

if ($StartLine -lt 1 -or $StartLine -gt $totalLines + 1) {
    Write-Error "StartLine $StartLine is out of bounds (1..$($totalLines + 1))."
    exit 1
}

if ($EndLine -lt $StartLine - 1) {
    Write-Error "EndLine $EndLine cannot be less than StartLine - 1 ($($StartLine - 1))."
    exit 1
}

$before = if ($StartLine -gt 1) { @($lines[0..($StartLine - 2)]) } else { @() }
$after = if ($EndLine -lt $totalLines) { @($lines[$EndLine..($totalLines - 1)]) } else { @() }

$result = @($before) + @($replacement) + @($after)
[System.IO.File]::WriteAllLines((Resolve-Path $Path).Path, $result, [System.Text.Encoding]::UTF8)

Write-Host "Successfully replaced lines $StartLine..$EndLine in $Path (New total lines: $($result.Length))."
