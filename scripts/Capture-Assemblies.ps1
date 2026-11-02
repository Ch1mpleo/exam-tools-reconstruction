[CmdletBinding()]
param([ValidateSet('EOS','PEA-CSD','PEA-PRN')][string]$Build)
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$rea = (Get-Command npx.cmd -ErrorAction Stop).Source
$reaPrefix = @('-y','rea-agents@6.8.0')
$ilspy = Join-Path $root '.tools\ilspycmd.cmd'
$entries = Get-Content (Join-Path $root 'exam-input-manifest.json') -Raw | ConvertFrom-Json
New-Item -ItemType Directory -Path (Join-Path $root 'evidence') -Force | Out-Null
$names = @('EOSClient.exe','PEALogin.exe','IRemote.dll','EncryptData.dll','QuestionLib.dll','GradeLib.dll','HashCodeProvider.dll')
foreach ($entry in $entries | Where-Object { $_.Build -eq $Build -and (Split-Path $_.Path -Leaf) -in $names }) {
  $entry.Path = Join-Path $root $entry.Path
  $name = [IO.Path]::GetFileNameWithoutExtension($entry.Path)
  $id = "$Build.$name"
  $out = Join-Path $root "decompiled\$Build\$name"
  New-Item -ItemType Directory -Force $out | Out-Null
  $members = Join-Path $root "evidence\$id.members.json"
  if (-not (Test-Path $members)) {
    & $rea @reaPrefix inspect-managed-members $entry.Path --format json > $members
    if ($LASTEXITCODE -ne 0) { throw "REA failed: $id" }
  }
  & $rea @reaPrefix inspect-managed-artifact $entry.Path --format json > (Join-Path $root "evidence\$id.artifact.json")
  if ($LASTEXITCODE -ne 0) { throw "REA identity failed: $id" }
  & $ilspy --disable-updatecheck -p -usepdb -o $out $entry.Path 2> (Join-Path $root "evidence\$id.decompile.log")
  $decompileExit = $LASTEXITCODE
  if ($decompileExit -ne 0 -and $name -ne 'EOSClient') { throw "Decompile failed: $id" }
  & $ilspy --disable-updatecheck -il $entry.Path > (Join-Path $root "evidence\$id.il")
  if ($LASTEXITCODE -ne 0) { throw "IL export failed: $id" }
  [pscustomobject]@{Build=$Build; Assembly=$name; DecompileExit=$decompileExit; ILExit=$LASTEXITCODE}
}
