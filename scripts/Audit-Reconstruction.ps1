$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
if (-not (Test-Path -LiteralPath (Join-Path $root 'evidence\EOS-normalized\EOSClient.analysis.exe')) -or
    -not (Test-Path -LiteralPath (Join-Path $root 'evidence\source-projects.json'))) {
  throw 'Local analysis evidence is missing. Run scripts/Reconstruct.ps1 once after importing the original packs; it also runs this audit.'
}
& (Join-Path $PSScriptRoot 'Build-Reconstruction.ps1')
& dotnet build (Join-Path $root 'tooling\Reconstruction.Tool') -v quiet
if ($LASTEXITCODE -ne 0) {throw 'Audit tooling build failed'}
& dotnet build (Join-Path $root 'tooling\LibraryProbe') -v quiet
if ($LASTEXITCODE -ne 0) {throw 'Probe build failed'}
$tool = Join-Path $root 'tooling\Reconstruction.Tool\bin\Debug\net10.0\Reconstruction.Tool.dll'
& dotnet $tool audit $root
if ($LASTEXITCODE -ne 0) {throw 'Static audit failed'}
foreach ($pack in @('EOS','PEA-CSD','PEA-PRN')) {
  foreach ($mode in @('original','rebuilt')) {
    & (Join-Path $root 'tooling\LibraryProbe\bin\Debug\net48\LibraryProbe.exe') $root $pack $mode
    if ($LASTEXITCODE -ne 0) {throw "Helper probe failed: $pack / $mode"}
  }
}
& dotnet $tool login-audit $root
if ($LASTEXITCODE -ne 0) {throw 'Login simulation failed'}
& python (Join-Path $root 'scripts\index_methods.py')
if ($LASTEXITCODE -ne 0) {throw 'Method catalog failed'}
& python (Join-Path $root 'scripts\audit_pea_logging.py')
if ($LASTEXITCODE -ne 0) {throw 'PEA logging reference audit failed'}
& python (Join-Path $root 'scripts\audit_report.py')
if ($LASTEXITCODE -ne 0) {throw 'Audit comparison failed'}
