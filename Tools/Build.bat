@echo off

pushd ..\Source
msbuild.exe Monotone.csproj /target:Build /property:Configuration=Release
if exist obj rd /q /s obj
popd

if exist ..\Build\Monotone.dll del ..\Build\Monotone.dll
if exist ..\Build\AppManifest.xaml del ..\Build\AppManifest.xaml
