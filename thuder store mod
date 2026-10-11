@echo off
REM Thunderstore mod package builder for Peak Summit Casino
REM This script creates the proper folder structure and zips it for Thunderstore

setlocal enabledelayedexpansion

REM Configuration
set MOD_NAME=PeakSummitCasino
set MOD_VERSION=0.1.0-alpha.1
set DLL_SOURCE=C:\Program Files (x86)\Steam\steamapps\common\PEAK\BepInEx\plugins\PeakSummitCasino.dll
set OUTPUT_DIR=%USERPROFILE%\Desktop
set TEMP_DIR=%TEMP%\%MOD_NAME%-build

REM Create temporary build directory
if exist "%TEMP_DIR%" rmdir /s /q "%TEMP_DIR%"
mkdir "%TEMP_DIR%\%MOD_NAME%-%MOD_VERSION%\plugins"

REM Copy DLL
if not exist "%DLL_SOURCE%" (
    echo ERROR: DLL not found at %DLL_SOURCE%
    exit /b 1
)
copy "%DLL_SOURCE%" "%TEMP_DIR%\%MOD_NAME%-%MOD_VERSION%\plugins\"

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
) > "%TEMP_DIR%\%MOD_NAME%-%MOD_VERSION%\manifest.json"

REM Create README.md
(
echo # Peak Summit Casino
echo.
echo A BepInEx multiplayer mod for PEAK that adds a gambling room at the summit with time-limited games from Gamble With Your Friends.
echo.
echo ## Installation
echo.
echo 1. Install BepInEx
echo 2. Put the DLL in your `BepInEx/plugins/` folder
echo 3. Launch PEAK
echo.
echo ## GitHub
echo.
echo https://github.com/jonnyboy0715-lab/peak-summit-casino
) > "%TEMP_DIR%\%MOD_NAME%-%MOD_VERSION%\README.md"

REM Create the zip file using PowerShell (Windows 10+)
powershell -Command "Add-Type -AssemblyName System.IO.Compression.FileSystem; [System.IO.Compression.ZipFile]::CreateFromDirectory('%TEMP_DIR%', '%OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip')"

REM Check if zip was created successfully
if exist "%OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip" (
    echo.
    echo SUCCESS! Thunderstore mod created:
    echo %OUTPUT_DIR%\%MOD_NAME%-%MOD_VERSION%.zip
    echo.
    echo You can now upload this to Thunderstore!
    explorer "%OUTPUT_DIR%"
) else (
    echo ERROR: Failed to create zip file
    exit /b 1
)

REM Cleanup
rmdir /s /q "%TEMP_DIR%"

pause
