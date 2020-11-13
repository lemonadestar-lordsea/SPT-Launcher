# Build.ps1
# Copyright: © SPT-AKI 2020
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

#setup variables
$buildDir = "Build/"
$launcherData = "./SPTarkov.Launcher/Launcher_Data/"

Write-Host $launcherData

# build the project
Write-Host "Building launcher ..." -ForegroundColor Cyan
$buildProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList "build --output ${buildDir} --configuration Release --verbosity minimal --nologo --no-incremental"
Wait-Process -InputObject $buildProcess
Write-Host ""
Write-Host "Done" -ForegroundColor Cyan

#copy launcher_data folder
Write-Host ""
Write-Host "Copying Launcher_Data folder ... " -NoNewLine

Copy-Item -Path $launcherData -Destination "./${buildDir}/Launcher_Data" -Recurse -Force -ErrorAction SilentlyContinue

if (Test-Path "$($buildDir)/Launcher_Data") 
{
    Write-host "OK" -ForegroundColor Green
}
else 
{
    Write-host "Folder doesn't appear to have been copied.`nError: $($Error[0])" -ForegroundColor Red
}

Write-Host ""

# delete build waste
Write-Host "Cleaning garbage produced by build..." -ForegroundColor Cyan

[string[]]$delPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin"} | select -ExpandProperty FullName
$delPaths += Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\obj"} | select -ExpandProperty FullName

foreach ($path in $delPaths)
{
    Write-Host "  Delete: $($path)"
    Remove-Item $path -Force -Recurse
}

Write-Host ""
Write-Host "Done" -ForegroundColor Cyan