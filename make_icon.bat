@echo off
REM Create casino-themed icon.png for Thunderstore
REM Converts sketch image to PNG icon (256x256)

setlocal enabledelayedexpansion

set OUTPUT=%CD%\icon.png

REM Create PNG using PowerShell with enhanced casino design
powershell -NoProfile -Command ^
"^
Add-Type -AssemblyName System.Drawing; ^
Add-Type -AssemblyName System.Windows.Forms; ^
^
$bitmap = New-Object System.Drawing.Bitmap(256, 256); ^
$graphics = [System.Drawing.Graphics]::FromImage($bitmap); ^
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias; ^
^
$graphics.Clear([System.Drawing.Color]::White); ^
^
$purple = [System.Drawing.Color]::FromArgb(156, 74, 170); ^
$darkPurple = [System.Drawing.Color]::FromArgb(100, 40, 120); ^
$gold = [System.Drawing.Color]::FromArgb(255, 201, 88); ^
^
$thickPen = New-Object System.Drawing.Pen($purple, 6); ^
$thickPen.LineJoin = [System.Drawing.Drawing2D.LineJoin]::Round; ^
$thinPen = New-Object System.Drawing.Pen($purple, 3); ^
^
$brush = New-Object System.Drawing.SolidBrush($purple); ^
^
$graphics.DrawRectangle($thickPen, 40, 45, 176, 166); ^
^
$graphics.DrawLine($thickPen, 52, 65, 204, 65); ^
$graphics.DrawLine($thickPen, 52, 130, 204, 130); ^
$graphics.DrawLine($thickPen, 52, 195, 204, 195); ^
^
$graphics.DrawRectangle($thinPen, 65, 80, 40, 35); ^
$graphics.DrawLine($thinPen, 75, 92, 95, 92); ^
$graphics.DrawLine($thinPen, 85, 82, 85, 118); ^
^
$graphics.DrawRectangle($thinPen, 151, 80, 40, 35); ^
$graphics.DrawLine($thinPen, 161, 92, 181, 92); ^
$graphics.DrawLine($thinPen, 171, 82, 171, 118); ^
^
$graphics.DrawRectangle($thinPen, 65, 145, 40, 35); ^
$graphics.DrawLine($thinPen, 75, 157, 95, 157); ^
$graphics.DrawLine($thinPen, 85, 147, 85, 183); ^
^
$graphics.DrawRectangle($thinPen, 151, 145, 40, 35); ^
$graphics.DrawLine($thinPen, 161, 157, 181, 157); ^
$graphics.DrawLine($thinPen, 171, 147, 171, 183); ^
^
$goldBrush = New-Object System.Drawing.SolidBrush($gold); ^
$graphics.FillEllipse($goldBrush, 110, 105, 36, 30); ^
$graphics.DrawEllipse($thickPen, 110, 105, 36, 30); ^
^
$bitmap.Save('%OUTPUT%', [System.Drawing.Imaging.ImageFormat]::Png); ^
$graphics.Dispose(); ^
$bitmap.Dispose(); ^
Write-Host 'Success: icon.png created' ^
"

if exist "%OUTPUT%" (
    echo.
    echo =========================================
    echo SUCCESS! Real PNG file created:
    echo %OUTPUT%
    echo =========================================
    echo.
) else (
    echo ERROR: Failed to create icon.png
    exit /b 1
)

pause
