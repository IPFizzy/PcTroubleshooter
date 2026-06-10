# Keon Bushman
# PC Troubleshooter Launcher
# Downloads and runs the latest temporary release of PC Troubleshooter.

$ErrorActionPreference = "Stop"

# Change these values to match your GitHub repo.
$RepoOwner = "IPFizzy"
$RepoName = "PcTroubleshooter"
$AssetName = "PcTroubleshooter-win-x64.zip"

$InstallRoot = Join-Path $env:TEMP "PcTroubleshooter"
$ZipPath = Join-Path $env:TEMP $AssetName
$ExePath = Join-Path $InstallRoot "PcTroubleshooter.exe"

Write-Host ""
Write-Host "PC Troubleshooter"
Write-Host "----------------"
Write-Host "Preparing temporary local copy..."
Write-Host ""

Remove-Item $InstallRoot -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $ZipPath -Force -ErrorAction SilentlyContinue

New-Item -ItemType Directory -Path $InstallRoot | Out-Null

Write-Host "Finding latest release..."
$ReleaseApiUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/releases/latest"
$Release = Invoke-RestMethod -Uri $ReleaseApiUrl

$Asset = $Release.assets | Where-Object { $_.name -eq $AssetName } | Select-Object -First 1

if ($null -eq $Asset) {
    Write-Host "Could not find release asset: $AssetName"
    Write-Host "Check your GitHub release upload."
    pause
    exit 1
}

Write-Host "Downloading latest tool package..."
Invoke-WebRequest -Uri $Asset.browser_download_url -OutFile $ZipPath

Write-Host "Extracting temporary files..."
Expand-Archive -Path $ZipPath -DestinationPath $InstallRoot -Force

if (!(Test-Path $ExePath)) {
    Write-Host "Could not find executable:"
    Write-Host $ExePath
    Write-Host ""
    Write-Host "Make sure PcTroubleshooter.exe is at the top level of the zip file."
    pause
    exit 1
}

Write-Host "Starting PC Troubleshooter..."
Start-Process -FilePath $ExePath -WorkingDirectory $InstallRoot

Write-Host "Done."
Write-Host ""
