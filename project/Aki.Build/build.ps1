# VSCode.ps1
# Copyright: © SPT-AKI 2021
# License: NCSA
# Authors:
# - Merijn Hendriks
# - waffle.lord

param (
        [switch]$VSBuilt,
        [string]$Config = "Release"
    )

#setup variables
Write-Host "Running build for config: $($Config)" -ForegroundColor Cyan
$buildDir = "Build/"
$debugDir = "./Aki.Launcher/bin/Debug/net6.0/win10-x64"
$launcherData = "./Aki.Launcher/Aki_Data/"
$publishSwitches = "--nologo --verbosity minimal --runtime win10-x64 --configuration $($Config) -p:PublishSingleFile=true --no-self-contained"
$pathPrefix = "./"

if($VSBuilt) {
    $buildDir = "../Build"
    $launcherData = "../Aki.Launcher/Aki_Data"
    $debugDir = "../Aki.Launcher/bin/Debug/net6.0/win10-x64"
    $pathPrefix = "../"

    if($Config -eq "Release") {
        $publishSwitches += " --no-build"
    }
}

#$debugDir = Resolve-Path $debugDir

$publishSwitches += " --output ${buildDir}"
$publishArgs = "publish " + $pathPrefix + "Aki.Launcher/Aki.Launcher.csproj " + $publishSwitches
$publishCLIArgs = "publish " + $pathPrefix + "Aki.Launcher.CLI/Aki.Launcher.CLI.csproj " + $publishSwitches

#removes the Obj and Bin directories and their contents
function CleanBuild 
{
    [string[]]$delPaths = Get-ChildItem -Recurse | where {$_.FullName -like "*\bin" -or $_.FullName -like "*\obj"} | select -ExpandProperty FullName

    foreach ($path in $delPaths)
    {
        Write-Host "  Delete: $($path)"

        #Remove the ErrorAction if you suspect something isn't being removed properly
        Remove-Item $path -Force -Recurse -ErrorAction SilentlyContinue
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
if(-not $VSBuilt)
{
    CleanBuild

    $cleanProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList "clean --nologo --verbosity quiet --runtime win10-x64"
    Wait-Process -InputObject $cleanProcess
    $cleanProcess.Dispose()
}

Write-Host "`nDone" -ForegroundColor Cyan

Write-Host "`nBuilding launcher ..." -ForegroundColor Cyan
$publishProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList $publishArgs
Wait-Process -InputObject $publishProcess
$publishProcess.Dispose()
Write-Host "`nDone" -ForegroundColor Cyan

Write-Host "`nBuilding launcher CLI..." -ForegroundColor Cyan
$publishProcess = Start-Process "dotnet" -PassThru -NoNewWindow -ArgumentList $publishCLIArgs
Wait-Process -InputObject $publishProcess
$publishProcess.Dispose()
Write-Host "`nDone" -ForegroundColor Cyan

if($Config -eq "Release") 
{
    Remove-Item "$($buildDir)\Aki.Launcher.pdb"
    Remove-Item "$($buildDir)\LauncherCLI.pdb"
}

Remove-Item "$($buildDir)\Aki.ByteBanger.pdb"
Remove-Item "$($buildDir)\Aki.Launcher.Base.pdb"

#copy aki_data folder for debugging
if($Config -eq "Debug") 
{
    Write-Host "`nCopying Aki_Data folder to debug ... " -NoNewLine

    if($debugDir -eq $null) 
    {
        Write-Host "Output path is null" -ForegroundColor Red
        break
    }

    $debugAkiDataDir = "$($debugDir)\Aki_Data"

    Copy-Item -Path $launcherData -Destination $debugAkiDataDir -Recurse -Force -ErrorAction SilentlyContinue

    if(Test-Path $debugAkiDataDir) 
    {
        Write-Host "OK" -ForegroundColor Green
    }
    else {
        Write-Host "Failed to copy Aki_Data to debug" -ForegroundColor Red
    }
}

#copy aki_data folder to Build
Write-Host "`nCopying Aki_Data folder to build ... " -NoNewLine

Copy-Item -Path $launcherData -Destination "./${buildDir}/Aki_Data" -Recurse -Force -ErrorAction SilentlyContinue

if (Test-Path "$($buildDir)/Aki_Data") 
{
    Write-host "OK" -ForegroundColor Green
}
else 
{
    Write-host "Folder doesn't appear to have been copied.`nError: $($Error[0])" -ForegroundColor Red
}

#Additionally copy the license file into the build folder
Write-Host "`nCopying license file ... " -NoNewLine
$LicenseFilePath = "$($buildDir)/../../LICENSE.md"
if (Test-Path $LicenseFilePath)
{
    Copy-Item -Path $LicenseFilePath -Destination "$($buildDir)/LICENSE-Launcher.txt" -Force -ErrorAction SilentlyContinue

    #check license has been copied
    if (Test-Path "$($buildDir)/LICENSE-Launcher.txt")
    {
        Write-host "OK" -ForegroundColor Green
    }
    else 
    {
        Write-host "Failed to copy license file" -ForegroundColor Red
    }
}
else
{
    Write-Warning "LICENSE.md file not found. If you're making a release, please don't forget to include the license file!"
}

# delete build waste if not debug

if($Config -ne "Debug") 
{
    Write-Host "`nCleaning garbage produced by build..." -ForegroundColor Cyan

    CleanBuild

    Write-Host "`nDone" -ForegroundColor Cyan
}