@echo off
REM Create icon.png for Thunderstore
REM This script generates a 256x256 PNG icon file

setlocal enabledelayedexpansion

set OUTPUT=%CD%\icon.png

REM Create PNG using PowerShell
powershell -NoProfile -Command ^
"^
Add-Type -AssemblyName System.Drawing; ^
$bitmap = New-Object System.Drawing.Bitmap(256, 256); ^
$graphics = [System.Drawing.Graphics]::FromImage($bitmap); ^
$graphics.Clear([System.Drawing.Color]::White); ^
^
$purplePen = New-Object System.Drawing.Pen([System.Drawing.Color]::FromArgb(156, 74, 170), 4); ^
$purpleBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(156, 74, 170)); ^
^
$graphics.DrawRectangle($purplePen, 36, 50, 184, 154); ^
$graphics.DrawLine($purplePen, 48, 58, 112, 58); ^
$graphics.DrawLine($purplePen, 114, 58, 186, 58); ^
$graphics.DrawLine($purplePen, 188, 58, 206, 58); ^
$graphics.DrawLine($purplePen, 66, 88, 66, 166); ^
$graphics.DrawLine($purplePen, 66, 166, 120, 166); ^
$graphics.DrawLine($purplePen, 120, 166, 180, 166); ^
$graphics.DrawLine($purplePen, 180, 166, 196, 166); ^
^
$graphics.DrawRectangle($purplePen, 78, 106, 52, 50); ^
$graphics.DrawLine($purplePen, 90, 126, 118, 126); ^
$graphics.DrawLine($purplePen, 104, 108, 104, 154); ^
^
$graphics.DrawRectangle($purplePen, 156, 106, 50, 50); ^
$graphics.DrawLine($purplePen, 168, 126, 194, 126); ^
$graphics.DrawLine($purplePen, 181, 108, 181, 154); ^
^
$bitmap.Save('%OUTPUT%', [System.Drawing.Imaging.ImageFormat]::Png); ^
$graphics.Dispose(); ^
$bitmap.Dispose(); ^
Write-Host 'Created: %OUTPUT%' ^
"

if exist "%OUTPUT%" (
    echo.
    echo SUCCESS! Created icon.png
    echo Location: %OUTPUT%
) else (
    echo ERROR: Failed to create icon.png
    exit /b 1
)

pause
