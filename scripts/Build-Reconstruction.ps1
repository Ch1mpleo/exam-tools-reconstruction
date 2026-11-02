$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
New-Item -ItemType Directory -Path (Join-Path $root 'evidence') -Force | Out-Null
foreach ($relative in @('inputs\EOS\NAudio.dll','inputs\EOS\NHibernate.dll','inputs\PEA-CSD\Ionic.Zip.dll','inputs\PEA-PRN\Ionic.Zip.dll')) {
  if (-not (Test-Path -LiteralPath (Join-Path $root $relative))) {
    throw "Missing $relative. Run scripts/Import-Inputs.ps1 with the supplied pack directories first."
  }
}
$projects = @('src\PEA-CSD\PEALogin','src\PEA-PRN\PEALogin','src\EOS\EOSClient','src\EOS\HashCodeProvider')
$results = foreach ($project in $projects) {
  $label = ($project -replace '^src\\','') -replace '\\','-'
  $log = Join-Path $root "evidence\build-$label.log"
  & dotnet build (Join-Path $root $project) -v minimal *> $log
  [pscustomobject]@{Project=$project;ExitCode=$LASTEXITCODE;Log=$log}
}
$results | ConvertTo-Json | Set-Content (Join-Path $root 'evidence\build-results.json')
$results | Format-Table -AutoSize
if ($results.ExitCode | Where-Object {$_ -ne 0}) {throw 'A reconstruction project did not compile. See evidence/build-results.json.'}
