@echo off

set FXC="C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe"

echo.
echo Kompiliere Mandelbrot.fx...
%FXC% /T ps_3_0 /E main /Fo Mandelbrot.ps Mandelbrot.fx

if errorlevel 1 goto :fehler

echo.
echo Kompiliere MandelbrotPerturbation.fx...
%FXC% /T ps_3_0 /E main /Fo MandelbrotPerturbation.ps MandelbrotPerturbation.fx

if errorlevel 1 goto :fehler

echo.
echo Kompiliere MultipassTestPass1.fx...
%FXC% /T ps_3_0 /E main /Fo MultipassTestPass1.ps MultipassTestPass1.fx

if errorlevel 1 goto :fehler

echo.
echo Kompiliere MultipassTestPass2.fx...
%FXC% /T ps_3_0 /E main /Fo MultipassTestPass2.ps MultipassTestPass2.fx

if errorlevel 1 goto :fehler

echo.
echo Alle Shader wurden erfolgreich kompiliert.
goto :ende

:fehler

echo.
echo FEHLER BEIM KOMPILIEREN DER SHADER!

:ende

echo.
pause