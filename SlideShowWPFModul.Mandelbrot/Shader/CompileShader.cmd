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
echo Kompiliere MPPValidationSeed.fx...
%FXC% /T ps_3_0 /E main /Fo MPPValidationSeed.ps MPPValidationSeed.fx

if errorlevel 1 goto :fehler

echo.
echo Kompiliere MPPValidationContinue.fx...
%FXC% /T ps_3_0 /E main /Fo MPPValidationContinue.ps MPPValidationContinue.fx

if errorlevel 1 goto :fehler

echo.
echo Kompiliere MPPValidationOutput.fx...
%FXC% /T ps_3_0 /E main /Fo MPPValidationOutput.ps MPPValidationOutput.fx

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