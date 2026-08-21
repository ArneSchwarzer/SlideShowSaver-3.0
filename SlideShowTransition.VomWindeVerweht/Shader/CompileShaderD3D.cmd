@echo off
setlocal

set FXC=C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe

del /Q MosaikShaderVS.cso 2>nul
del /Q MosaikShaderPS.cso 2>nul

"%FXC%" /T vs_5_0 /E VSMain /Fo MosaikShaderVS.cso MosaikShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo MosaikShaderPS.cso MosaikShader.hlsl
if errorlevel 1 goto error

echo.
echo Direct3D-Shader erfolgreich kompiliert.
goto ende

:error
echo.
echo FEHLER BEIM KOMPILIEREN!

:ende
pause