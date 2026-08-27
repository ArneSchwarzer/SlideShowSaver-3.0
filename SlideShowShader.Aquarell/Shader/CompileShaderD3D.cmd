@echo off
setlocal

set FXC=C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe

del /Q AquarellCopyShaderVS.cso 2>nul
del /Q AquarellCopyShaderPS.cso 2>nul

del /Q BilateralShaderVS.cso 2>nul
del /Q BilateralShaderPS.cso 2>nul

del /Q KuwaharaShaderVS.cso 2>nul
del /Q KuwaharaShaderPS.cso 2>nul

del /Q AniKuwaharaShaderVS.cso 2>nul
del /Q AniKuwaharaShaderPS.cso 2>nul

"%FXC%" /T vs_5_0 /E VSMain /Fo AquarellCopyShaderVS.cso AquarellCopyShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo AquarellCopyShaderPS.cso AquarellCopyShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo BilateralShaderVS.cso BilateralShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo BilateralShaderPS.cso BilateralShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo KuwaharaShaderVS.cso KuwaharaShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo KuwaharaShaderPS.cso KuwaharaShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T vs_5_0 /E VSMain /Fo AniKuwaharaShaderVS.cso AniKuwaharaShader.hlsl
if errorlevel 1 goto error

"%FXC%" /T ps_5_0 /E PSMain /Fo AniKuwaharaShaderPS.cso AniKuwaharaShader.hlsl
if errorlevel 1 goto error


echo.
echo Direct3D-Shader erfolgreich kompiliert.
goto ende

:error
echo.
echo FEHLER BEIM KOMPILIEREN!

:ende
pause