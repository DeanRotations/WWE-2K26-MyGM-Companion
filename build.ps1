param([string]$OutputDirectory)
$ErrorActionPreference = 'Stop'
$src = Join-Path $PSScriptRoot 'src'
$stage = Join-Path $PSScriptRoot 'stage'
$updateStage = Join-Path $PSScriptRoot 'update-stage'
$out = if ($OutputDirectory) { [IO.Path]::GetFullPath($OutputDirectory) } else { Join-Path (Split-Path $PSScriptRoot -Parent) 'outputs' }
if (Test-Path $stage) { Remove-Item -LiteralPath $stage -Recurse -Force }
if (Test-Path $updateStage) { Remove-Item -LiteralPath $updateStage -Recurse -Force }
New-Item -ItemType Directory -Path $stage -Force | Out-Null
New-Item -ItemType Directory -Path $updateStage -Force | Out-Null
New-Item -ItemType Directory -Path $out -Force | Out-Null
dotnet publish (Join-Path $src 'MyGM.Companion\MyGM.Companion.csproj') -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o $stage
if ($LASTEXITCODE -ne 0) { throw 'Companion build failed.' }
dotnet publish (Join-Path $src 'MyGM.OcrWorker\MyGM.OcrWorker.csproj') -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o $stage
if ($LASTEXITCODE -ne 0) { throw 'OCR worker build failed.' }
dotnet publish (Join-Path $src 'MyGM.Updater\MyGM.Updater.csproj') -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o $stage
if ($LASTEXITCODE -ne 0) { throw 'Updater build failed.' }
dotnet publish (Join-Path $src 'MyGM.Uninstaller\MyGM.Uninstaller.csproj') -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o $stage
if ($LASTEXITCODE -ne 0) { throw 'Uninstaller build failed.' }
foreach ($dir in @('assets','config','data','cache','logs')) {
  $sourceDir = Join-Path $src "MyGM.Companion\$dir"
  $targetDir = Join-Path $stage $dir
  New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
  if (Test-Path $sourceDir) { Copy-Item (Join-Path $sourceDir '*') $targetDir -Recurse -Force }
}
Copy-Item (Join-Path $PSScriptRoot 'CHANGELOG.md') (Join-Path $stage 'CHANGELOG.md') -Force
Copy-Item (Join-Path $stage '*') $updateStage -Recurse -Force
foreach ($preserved in @('data','config','cache','logs')) {
  $preservedPath = Join-Path $updateStage $preserved
  if (Test-Path $preservedPath) { Remove-Item -LiteralPath $preservedPath -Recurse -Force }
}
Compress-Archive -Path (Join-Path $updateStage '*') -DestinationPath (Join-Path $out 'MyGMCompanion-update.zip') -Force
$updateHash = (Get-FileHash (Join-Path $out 'MyGMCompanion-update.zip') -Algorithm SHA256).Hash
[IO.File]::WriteAllText((Join-Path $out 'MyGMCompanion-update.zip.sha256'), "$updateHash  MyGMCompanion-update.zip`r`n")
Compress-Archive -Path (Join-Path $stage '*') -DestinationPath (Join-Path $src 'MyGM.Setup\payload.zip') -Force
dotnet publish (Join-Path $src 'MyGM.Setup\MyGM.Setup.csproj') -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o (Join-Path $PSScriptRoot 'setup-publish')
if ($LASTEXITCODE -ne 0) { throw 'Setup build failed.' }
Copy-Item (Join-Path $PSScriptRoot 'setup-publish\Setup.exe') (Join-Path $out 'WWE-2K26-MyGM-Companion-V12.0.0-Setup.exe') -Force
Copy-Item (Join-Path $PSScriptRoot 'CHANGELOG.md') (Join-Path $out 'CHANGELOG-V12.0.0.md') -Force
Get-FileHash (Join-Path $out 'WWE-2K26-MyGM-Companion-V12.0.0-Setup.exe') -Algorithm SHA256 | Format-List
