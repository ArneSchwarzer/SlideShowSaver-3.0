@echo off
setlocal

set FXC=C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe

del /Q RevealShaderVS.cso 2>nul
del /Q RevealShaderPS.cso 2>nul

del /Q GradientenShaderVS.cso 2>nul
del /Q GradientenShaderPS.cso 2>nul

"%FXC%" /T vs_5_0 /E VSMain /Fo RevealShaderVS.cso RevealShader.hlsl

if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo RevealShaderPS.cso RevealShader.hlsl

if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo GradientenShaderVS.cso GradientenShader.hlsl

if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo GradientenShaderPS.cso GradientenShader.hlsl

if errorlevel 1 goto error

echo.
echo Direct3D-Shader erfolgreich kompiliert.
goto ende

:error
echo.
echo FEHLER BEIM KOMPILIEREN!

:ende
pause