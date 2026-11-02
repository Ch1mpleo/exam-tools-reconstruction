[CmdletBinding()]
param([switch]$RefreshEvidence)
$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
$before=Get-Location
try {
 Set-Location -LiteralPath $root
 $entries=Get-Content .\exam-input-manifest.json -Raw | ConvertFrom-Json
 foreach($e in $entries){$e.Path=Join-Path $root $e.Path;if((Get-FileHash -LiteralPath $e.Path -Algorithm SHA256).Hash -ne $e.SHA256){throw "Input changed: $($e.Path)"}}
 New-Item -ItemType Directory -Path (Join-Path $root 'evidence') -Force | Out-Null
 $ilspy=Join-Path $root '.tools\ilspycmd.cmd'
 if(-not(Test-Path -LiteralPath $ilspy)){& dotnet tool install ilspycmd --version 11.2.0.10050 --tool-path (Join-Path $root '.tools');if($LASTEXITCODE -ne 0){throw 'ILSpy install failed'}}
 $captureNames=@('EOSClient.exe','PEALogin.exe','IRemote.dll','GradeLib.dll','EncryptData.dll','QuestionLib.dll','HashCodeProvider.dll')
 foreach($pack in @('EOS','PEA-CSD','PEA-PRN')){
  $missing=@($entries | Where-Object {$_.Build -eq $pack -and (Split-Path $_.Path -Leaf) -in $captureNames} | Where-Object {
   $stem=[IO.Path]::GetFileNameWithoutExtension($_.Path)
   -not(Test-Path -LiteralPath (Join-Path $root "decompiled\$pack\$stem\$stem.csproj")) -or -not(Test-Path -LiteralPath (Join-Path $root "evidence\$pack.$stem.members.json"))
  })
  if($RefreshEvidence -or $missing.Count){& .\scripts\Capture-Assemblies.ps1 -Build $pack}
 }
 & dotnet build .\tooling\Reconstruction.Tool -v quiet
 if($LASTEXITCODE -ne 0){throw 'Analysis tooling did not build'}
 $tool=Join-Path $root 'tooling\Reconstruction.Tool\bin\Debug\net10.0\Reconstruction.Tool.dll'
 $eos=($entries | Where-Object {$_.Path.EndsWith('EOSClient.exe')}).Path
 & dotnet $tool $eos .\evidence\EOS-normalized normalize
 if($LASTEXITCODE -ne 0){throw 'EOS normalization failed'}
 & $ilspy --disable-updatecheck --no-dead-stores -p -r (Split-Path $eos -Parent) -o .\src\EOS.Analysis .\evidence\EOS-normalized\EOSClient.analysis.exe
 if($LASTEXITCODE -ne 0){throw 'Normalized EOS decompile failed'}
 & python .\scripts\decode_eos_constants.py
 if($LASTEXITCODE -ne 0){throw 'Offline constants decode failed'}
 & $ilspy --disable-updatecheck -p -o .\decompiled\EOS\EmbeddedDependency .\evidence\EOS-normalized\embedded-dependency.dll
 if($LASTEXITCODE -ne 0){throw 'Packed resource export failed'}
 $names=@('EOSClient.exe','PEALogin.exe','IRemote.dll','GradeLib.dll','EncryptData.dll','QuestionLib.dll','HashCodeProvider.dll')
 foreach($e in $entries | Where-Object {(Split-Path $_.Path -Leaf) -in $names}){
  $name=[IO.Path]::GetFileNameWithoutExtension($e.Path);$inputPath=$e.Path;$output=Join-Path $root "evidence\method-exports\$($e.Build).$name"
  if($name -eq 'EOSClient'){$inputPath=Join-Path $root 'evidence\EOS-normalized\EOSClient.analysis.exe';$output=Join-Path $root 'evidence\EOS-normalized'}
  & dotnet $tool $inputPath $output export (Split-Path $e.Path -Parent)
  if($LASTEXITCODE -ne 0){throw "Method export failed: $name"}
 }
 & python .\scripts\prepare_readable_source.py
 if($LASTEXITCODE -ne 0){throw 'Source preparation failed'}
 & python .\scripts\index_methods.py
 if($LASTEXITCODE -ne 0){throw 'Method notes failed'}
 & python .\scripts\annotate_source.py
 if($LASTEXITCODE -ne 0){throw 'Source annotations failed'}
 & .\scripts\Audit-Reconstruction.ps1
} finally {Set-Location -LiteralPath $before.Path}
