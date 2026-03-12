[CmdletBinding()]
param(
    [string]$SourceDirectory = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\KommerceServer-Import\Gen\kamlbo')),
    [string]$DestinationDirectory = (Join-Path $PSScriptRoot 'kamlbo')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not (Test-Path -Path $SourceDirectory -PathType Container))
{
    throw "Source directory not found: $SourceDirectory"
}

if (-not (Test-Path -Path $DestinationDirectory -PathType Container))
{
    New-Item -Path $DestinationDirectory -ItemType Directory | Out-Null
}

$sourceFiles = @(
    Get-ChildItem -Path $SourceDirectory -Filter '*_kraken.kamlbo' -File
    Get-ChildItem -Path $SourceDirectory -Filter '*.kraken_kamlbo' -File
) | Sort-Object -Property FullName -Unique

foreach ($sourceFile in $sourceFiles)
{
    $destinationFileName = switch -Regex ($sourceFile.Name)
    {
        '^(?<base>.+)_kraken\.kamlbo$' { '{0}.kamlbo' -f $Matches['base']; break }
        '^(?<base>.+)\.kraken_kamlbo$' { '{0}.kamlbo' -f $Matches['base']; break }
        default { throw "Unsupported source file name format: $($sourceFile.Name)" }
    }

    $destinationPath = Join-Path $DestinationDirectory $destinationFileName
    Copy-Item -Path $sourceFile.FullName -Destination $destinationPath -Force
    Write-Host "Copied $($sourceFile.Name) -> $destinationFileName"
}

Write-Host "Copied $($sourceFiles.Count) file(s) to $DestinationDirectory"
