# Build.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

#setup variables
$buildDir = "Build/"
$launcherData = "./SPTarkov.Launcher/Launcher_Data/"

#removes the Obj and Bin directories and their contents
function CleanBuild 
{
    [string[]]$delPaths = Get-ChildItem -Recurse | where {$_.FullName -like "*\bin" -or $_.FullName -like "*\obj"} | select -ExpandProperty FullName

    foreach ($path in $delPaths)
    {
        Write-Host "  Delete: $($path)"
        Remove-Item $path -Force -Recurse
    }
}

# build the project
Write-Host "Cleaning previous builds ..." -ForegroundColor Cyan

#remove build dir to avoid reminant data
if (Test-Path $buildDir) 
{
    Remove-Item $buildDir -Recurse -Force
}

#make sure bin and obj don't exist before cleaning or errors may occur from previous builds ran from VS
CleanBuild

$cleanProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList "clean --nologo --runtime win10-x64 --verbosity quiet"
Wait-Process -InputObject $cleanProcess
$cleanProcess.Dispose()
Write-Host "`nDone" -ForegroundColor Cyan

Write-Host "`nBuilding launcher ..." -ForegroundColor Cyan
$publishProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList "publish --nologo --verbosity minimal --runtime win10-x64 -p:PublishSingleFile=true --configuration Release --output ${buildDir} --no-self-contained"
Wait-Process -InputObject $publishProcess
$publishProcess.Dispose()
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

CleanBuild

Write-Host "`nDone" -ForegroundColor Cyan