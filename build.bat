@echo off
REM Thunderstore mod package builder for Peak Summit Casino
REM This script creates the proper folder structure and zips it for Thunderstore

setlocal enabledelayedexpansion

REM Configuration
set MOD_NAME=PeakSummitCasino
set MOD_VERSION=0.1.0-alpha.1
set DLL_SOURCE=C:\Program Files (x86)\Steam\steamapps\common\PEAK\BepInEx\plugins\PeakSummitCasino.dll
set OUTPUT_DIR=%USERPROFILE%\Desktop
set PACKAGE_DIR=%TEMP%\%MOD_NAME%-%MOD_VERSION%-pkg

REM Create temporary package directory
if exist "%PACKAGE_DIR%" rmdir /s /q "%PACKAGE_DIR%"
mkdir "%PACKAGE_DIR%\plugins"

REM Copy DLL
if not exist "%DLL_SOURCE%" (
    echo ERROR: DLL not found at %DLL_SOURCE%
    pause
    exit /b 1
)
copy "%DLL_SOURCE%" "%PACKAGE_DIR%\plugins\" >nul
if %errorlevel% neq 0 (
    echo ERROR: Failed to copy DLL
    pause
    exit /b 1
)

REM Create manifest.json
(
echo {
echo   "name": "%MOD_NAME%",
echo   "version_number": "%MOD_VERSION%",
echo   "website_url": "https://github.com/jonnyboy0715-lab/peak-summit-casino",
echo   "description": "A BepInEx multiplayer mod for PEAK that adds a gambling room at the summit with time-limited games",
echo   "dependencies": [
echo     "BepInEx-BepInExPack_PEAK-5.4.9"
echo   ]
echo }
) > "%PACKAGE_DIR%\manifest.json"

REM Create README.md
(
echo # Peak Summit Casino
echo.
echo A BepInEx multiplayer mod for PEAK that adds a gambling room at the summit with time-limited games from Gamble With Your Friends.
echo.
echo ## Installation
echo.
echo 1. Install BepInEx
echo 2. Extract this mod into your `BepInEx/plugins/` folder
echo 3. Launch PEAK
echo.
echo ## Features
echo.
echo - Multiplayer gambling room
echo - Time-limited games
echo - Integration with Gamble With Your Friends mod
echo.
echo ## GitHub
echo.
echo https://github.com/jonnyboy0715-lab/peak-summit-casino
) > "%PACKAGE_DIR%\README.md"

REM Create a simple PNG icon (1x1 transparent PNG in base64)
REM This creates a minimal valid PNG file
powershell -Command "^
[byte[]]$png = @(137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 0, 1, 0, 0, 0, 1, 8, 6, 0, 0, 0, 31, 21, 196, 137, 0, 0, 0, 10, 73, 68, 65, 84, 120, 156, 63, 0, 1, 0, 0, 5, 0, 1, 13, 10, 45, 176, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130); ^
[System.IO.File]::WriteAllBytes('%PACKAGE_DIR%\icon.png', $png) ^
"

if %errorlevel% neq 0 (
    echo WARNING: Could not create icon.png, continuing anyway...
)

REM Delete existing zip if it exists
if exist "%OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip" (
    del "%OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip"
)

REM Create the zip file using PowerShell
powershell -Command "^
Add-Type -AssemblyName System.IO.Compression.FileSystem; ^
$packagePath = '%PACKAGE_DIR%'; ^
$zipPath = '%OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip'; ^
[System.IO.Compression.ZipFile]::CreateFromDirectory($packagePath, $zipPath, [System.IO.Compression.CompressionLevel]::Optimal, $false); ^
"

REM Check if zip was created successfully
if exist "%OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip" (
    echo.
    echo ========================================
    echo SUCCESS! Thunderstore mod created:
    echo %OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip
    echo.
    echo Contents:
    echo  - manifest.json
    echo  - icon.png
    echo  - README.md
    echo  - plugins\PeakSummitCasino.dll
    echo.
    echo Ready to upload to Thunderstore!
    echo ========================================
    echo.
    explorer "%OUTPUT_DIR%"
) else (
    echo ERROR: Failed to create zip file
    pause
    exit /b 1
)

REM Cleanup
rmdir /s /q "%PACKAGE_DIR%"

pause
