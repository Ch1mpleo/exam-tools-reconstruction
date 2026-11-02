[CmdletBinding()]
param(
  [Parameter(Mandatory)][string]$EOSPath,
  [Parameter(Mandatory)][string]$CSDPath,
  [Parameter(Mandatory)][string]$PRNPath
)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$folders = @{ 'EOS' = $EOSPath; 'PEA-CSD' = $CSDPath; 'PEA-PRN' = $PRNPath }
$entries = Get-Content (Join-Path $root 'exam-input-manifest.json') -Raw | ConvertFrom-Json
# Validate every source before copying anything. Keep pack-specific DLLs separate.
$copies = foreach ($entry in $entries) {
  $source = Join-Path $folders[$entry.Build] (Split-Path $entry.Path -Leaf)
  if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing input: $source" }
  if ((Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash -ne $entry.SHA256) {
    throw "This is a different build: $source"
  }
  [pscustomobject]@{ Source = (Resolve-Path -LiteralPath $source).Path; Destination = Join-Path $root $entry.Path }
}
foreach ($copy in $copies) {
  New-Item -ItemType Directory -Path (Split-Path $copy.Destination -Parent) -Force | Out-Null
  if ($copy.Source -ne [IO.Path]::GetFullPath($copy.Destination)) {
    Copy-Item -LiteralPath $copy.Source -Destination $copy.Destination -Force
  }
}
Write-Output "Imported $($copies.Count) verified files into inputs/. Original binaries are ignored by Git."
