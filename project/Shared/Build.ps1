# Build.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

#setup variables
$buildDir = "Build/"
$launcherData = "./SPTarkov.Launcher/Launcher_Data/"

# build the project
Write-Host "Cleaning previous builds ..." -ForegroundColor Cyan
$cleanProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList "clean --nologo --verbosity minimal"
Wait-Process -InputObject $cleanProcess

Write-Host "`nBuilding launcher ..." -ForegroundColor Cyan
$publishProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList "publish --nologo --verbosity minimal --runtime win10-x64 --configuration Release --output ${buildDir} --no-self-contained"
Wait-Process -InputObject $publishProcess
Write-Host "`nDone" -ForegroundColor Cyan

#copy launcher_data folder
Write-Host "`nCopying Launcher_Data folder ... " -NoNewLine

Copy-Item -Path $launcherData -Destination "./${buildDir}/Launcher_Data" -Recurse -Force -ErrorAction SilentlyContinue

if (Test-Path "$($buildDir)/Launcher_Data") 
{
    Write-host "OK" -ForegroundColor Green
}
else 
{
    Write-host "Folder doesn't appear to have been copied.`nError: $($Error[0])" -ForegroundColor Red
}

# delete build waste
Write-Host "`nCleaning garbage produced by build..." -ForegroundColor Cyan

[string[]]$delPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin"} | select -ExpandProperty FullName
$delPaths += Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\obj"} | select -ExpandProperty FullName

foreach ($path in $delPaths)
{
    Write-Host "  Delete: $($path)"
    Remove-Item $path -Force -Recurse
}

Write-Host "`nDone" -ForegroundColor Cyan