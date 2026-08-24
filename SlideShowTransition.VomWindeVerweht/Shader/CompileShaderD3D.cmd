@echo off
setlocal

set FXC=C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe

del /Q HintergrundShaderVS.cso 2>nul
del /Q HintergrundShaderPS.cso 2>nul

del /Q MosaikShaderVS.cso 2>nul
del /Q MosaikShaderPS.cso 2>nul

del /Q PartikelBewegungsShaderCS.cso 2>nul

del /Q MosaikShaderAPCVS.cso 2>nul
del /Q MosaikShaderAPCPS.cso 2>nul

del /Q PartikelBewegungsShaderAPCCS.cso 2>nul


"%FXC%" /T vs_5_0 /E VSMain /Fo HintergrundShaderVS.cso HintergrundShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo HintergrundShaderPS.cso HintergrundShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo MosaikShaderVS.cso MosaikShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo MosaikShaderPS.cso MosaikShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T cs_5_0 /E CSMain /Fo PartikelBewegungsShaderCS.cso PartikelBewegungsShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo MosaikShaderAPCVS.cso MosaikShaderAPC.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo MosaikShaderAPCPS.cso MosaikShaderAPC.hlsl
if errorlevel 1 goto error

"%FXC%" /T cs_5_0 /E CSMain /Fo PartikelBewegungsShaderAPCCS.cso PartikelBewegungsShaderAPC.hlsl
if errorlevel 1 goto error


echo.
echo Direct3D-Shader erfolgreich kompiliert.
goto ende

:error
echo.
echo FEHLER BEIM KOMPILIEREN!

:ende
pause