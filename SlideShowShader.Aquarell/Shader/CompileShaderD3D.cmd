@echo off
setlocal

set FXC=C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe

del /Q AquarellCopyShaderVS.cso 2>nul
del /Q AquarellCopyShaderPS.cso 2>nul

del /Q PigmentInitializerShaderVS.cso 2>nul
del /Q PigmentInitializerShaderPS.cso 2>nul

del /Q KuwaharaShaderVS.cso 2>nul
del /Q KuwaharaShaderPS.cso 2>nul

del /Q PaperInitializerShaderVS.cso 2>nul
del /Q PaperInitializerShaderPS.cso 2>nul

del /Q WaterInitializerShaderVS.cso 2>nul
del /Q WaterInitializerShaderPS.cso 2>nul

del /Q WaterFlowShaderVS.cso 2>nul
del /Q WaterFlowShaderPS.cso 2>nul

del /Q PigmentFlowShaderVS.cso 2>nul
del /Q PigmentFlowShaderPS.cso 2>nul

del /Q PigmentDisplayShaderVS.cso 2>nul
del /Q PigmentDisplayShaderPS.cso 2>nul


"%FXC%" /T vs_5_0 /E VSMain /Fo AquarellCopyShaderVS.cso AquarellCopyShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo AquarellCopyShaderPS.cso AquarellCopyShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo PigmentInitializerShaderVS.cso PigmentInitializerShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo PigmentInitializerShaderPS.cso PigmentInitializerShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo KuwaharaShaderVS.cso KuwaharaShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo KuwaharaShaderPS.cso KuwaharaShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo PaperInitializerShaderVS.cso PaperInitializerShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo PaperInitializerShaderPS.cso PaperInitializerShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo WaterInitializerShaderVS.cso WaterInitializerShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo WaterInitializerShaderPS.cso WaterInitializerShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo WaterFlowShaderVS.cso WaterFlowShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo WaterFlowShaderPS.cso WaterFlowShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo PigmentFlowShaderVS.cso PigmentFlowShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo PigmentFlowShaderPS.cso PigmentFlowShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo PigmentDisplayShaderVS.cso PigmentDisplayShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo PigmentDisplayShaderPS.cso PigmentDisplayShader.hlsl
if errorlevel 1 goto error


echo.
echo Direct3D-Shader erfolgreich kompiliert.
goto ende

:error
echo.
echo FEHLER BEIM KOMPILIEREN!

:ende
pause