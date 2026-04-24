param(
    [switch]$AttemptInstall
)

function Test-WinGet {
    try {
        & winget --version *> $null
        return ($LASTEXITCODE -eq 0)
    }
    catch {
        return $false
    }
}

$wingetDocsUrl = "https://learn.microsoft.com/en-us/windows/package-manager/winget/"

if (Test-WinGet) {
    Write-Host "WinGet is available."
    exit 0
}

if ($AttemptInstall) {
    Write-Host "WinGet is not available. Trying to enable or repair App Installer..."

    try {
        Add-AppxPackage -RegisterByFamilyName -MainPackage Microsoft.DesktopAppInstaller_8wekyb3d8bbwe -ErrorAction Stop
        Start-Sleep -Seconds 2
    }
    catch {
        Write-Host "Automatic App Installer registration failed: $($_.Exception.Message)"
    }
}

if (Test-WinGet) {
    Write-Host "WinGet is now available."
    exit 0
}

Write-Host "WinGet is still not available."
Write-Host "Install it manually from the official Microsoft page:"
Write-Host $wingetDocsUrl
exit 1
