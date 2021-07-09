# VS2019.ps1
# Copyright: © SPT-AKI 2021
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

#setup variables
$buildDir = "Build/"
$launcherData = "./Aki.Launcher/Aki_Data/"

# build the project
Write-Host "Cleaning previous builds ..." -ForegroundColor Cyan

#remove build dir to avoid reminant data
if (Test-Path $buildDir) 
{
    Remove-Item $buildDir -Recurse -Force
}

Remove-Item "$($buildDir)\Launcher.pdb"
Remove-Item "$($buildDir)\Aki.ByteBanger.pdb"

#copy aki_data folder
Write-Host "`nCopying Aki_Data folder ... " -NoNewLine

Copy-Item -Path $launcherData -Destination "./${buildDir}/Aki_Data" -Recurse -Force -ErrorAction SilentlyContinue

if (Test-Path "$($buildDir)/Aki_Data") 
{
    Write-host "OK" -ForegroundColor Green
}
else 
{
    Write-host "Folder doesn't appear to have been copied.`nError: $($Error[0])" -ForegroundColor Red
}

Write-Host "`nDone" -ForegroundColor Cyan