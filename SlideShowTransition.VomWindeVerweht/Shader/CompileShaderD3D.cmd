@echo off
setlocal

set FXC=C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe

del /Q HintergrundShaderVS.cso 2>nul
del /Q HintergrundShaderPS.cso 2>nul

del /Q PartikelBewegungsShaderAPCCS.cso 2>nul

del /Q MosaikVertexShaderFeinVS.cso 2>nul
del /Q MosaikVertexShaderMittelVS.cso 2>nul
del /Q MosaikVertexShaderGrobVS.cso 2>nul
del /Q MosaikPixelShaderPS.cso 2>nul

"%FXC%" /T vs_5_0 /E VSMain /Fo MosaikVertexShaderFeinVS.cso MosaikVertexShaderFein.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo MosaikVertexShaderMittelVS.cso MosaikVertexShaderMittel.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo MosaikVertexShaderGrobVS.cso MosaikVertexShaderGrob.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo MosaikPixelShaderPS.cso MosaikPixelShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo HintergrundShaderVS.cso HintergrundShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo HintergrundShaderPS.cso HintergrundShader.hlsl
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