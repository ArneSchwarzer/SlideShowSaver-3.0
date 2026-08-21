@echo off
setlocal EnableExtensions

:: ============================================================================
:: SlideShowSaver 3.0 - ModulExport
::
:: Kopiert die gebauten Module, Transitionen und Shader in das
:: Ausgabeverzeichnis von SlideShowMain und benennt die DLLs dabei in die
:: SlideShowSaver-spezifischen Dateiendungen um.
::
:: Zusaetzlich werden zentrale Laufzeitdaten wie die Ortsdatenbanken aus
:: SlideShowTools in das Ausgabeverzeichnis von SlideShowMain kopiert.
::
:: Aktuelle Zielarchitektur:
::     x64 / Debug
::
:: Muttern raeumt hier auf.
:: ============================================================================


:: ============================================================================
:: ZENTRALE KONFIGURATION
:: ============================================================================

set "ROOT=C:\Users\Arne\source\repos\SlideShowSaver 3.0"
set "PLATFORM=x64"
set "CONFIGURATION=Debug"

set "BUILD=%PLATFORM%\%CONFIGURATION%"
set "MAIN=%ROOT%\SlideShowMain\bin\%BUILD%"

set "TARGET_MODULE=%MAIN%\Module"
set "TARGET_TRANSITIONS=%MAIN%\Transitions"
set "TARGET_SHADER=%MAIN%\Shader"
set "TARGET_ORTE=%MAIN%\Orte"

set "SOURCE_ORTE=%ROOT%\SlideShowTools\Orte"


echo.
echo ============================================================================
echo SlideShowSaver 3.0 - ModulExport
echo ============================================================================
echo.
echo Architektur : %PLATFORM%
echo Konfiguration: %CONFIGURATION%
echo Ziel         : %MAIN%
echo.


:: ============================================================================
:: ZIELVERZEICHNISSE
:: ============================================================================

call :CreateDirectory "%TARGET_MODULE%"
if errorlevel 1 goto :Error

call :CreateDirectory "%TARGET_TRANSITIONS%"
if errorlevel 1 goto :Error

call :CreateDirectory "%TARGET_SHADER%"
if errorlevel 1 goto :Error

call :CreateDirectory "%TARGET_ORTE%"
if errorlevel 1 goto :Error


:: ============================================================================
:: MODULE
:: ============================================================================

echo.
echo ============================================================================
echo Module
echo ============================================================================
echo.

call :ExportPlugin ^
    "%ROOT%\SlideShowWPFModul.Mandelbrot\bin\%BUILD%\SlideShowWPFModul.Mandelbrot.dll" ^
    "%ROOT%\SlideShowWPFModul.Mandelbrot\bin\%BUILD%\SlideShowWPFModul.Mandelbrot.pdb" ^
    "%TARGET_MODULE%\Mandelbrot.sssm" ^
    "%TARGET_MODULE%\Mandelbrot.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowModul.Matrix\bin\%BUILD%\SlideShowModul.Matrix.dll" ^
    "%ROOT%\SlideShowModul.Matrix\bin\%BUILD%\SlideShowModul.Matrix.pdb" ^
    "%TARGET_MODULE%\Matrix.sssm" ^
    "%TARGET_MODULE%\Matrix.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowWPFModul.SlideShowSaver 3.0\bin\%BUILD%\SlideShowWPFModul.SlideShowSaver 3.0.dll" ^
    "%ROOT%\SlideShowWPFModul.SlideShowSaver 3.0\bin\%BUILD%\SlideShowWPFModul.SlideShowSaver 3.0.pdb" ^
    "%TARGET_MODULE%\SlideShowSaver 3.0.sssm" ^
    "%TARGET_MODULE%\SlideShowSaver 3.0.pdb"
if errorlevel 1 goto :Error


:: ============================================================================
:: TRANSITIONEN
:: ============================================================================

echo.
echo ============================================================================
echo Transitionen
echo ============================================================================
echo.

call :ExportPlugin ^
    "%ROOT%\SlideShowTransition.Cut\bin\%BUILD%\SlideShowTransition.Cut.dll" ^
    "%ROOT%\SlideShowTransition.Cut\bin\%BUILD%\SlideShowTransition.Cut.pdb" ^
    "%TARGET_TRANSITIONS%\Direkter Uebergang.ssst" ^
    "%TARGET_TRANSITIONS%\Direkter Uebergang.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowWPFTransition.SchiebenWischen\bin\%BUILD%\SlideShowWPFTransition.SchiebenWischen.dll" ^
    "%ROOT%\SlideShowWPFTransition.SchiebenWischen\bin\%BUILD%\SlideShowWPFTransition.SchiebenWischen.pdb" ^
    "%TARGET_TRANSITIONS%\Schieben und Wischen.ssst" ^
    "%TARGET_TRANSITIONS%\Schieben und Wischen.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowWPFTransition.Zoom\bin\%BUILD%\SlideShowWPFTransition.Zoom.dll" ^
    "%ROOT%\SlideShowWPFTransition.Zoom\bin\%BUILD%\SlideShowWPFTransition.Zoom.pdb" ^
    "%TARGET_TRANSITIONS%\Zoom.ssst" ^
    "%TARGET_TRANSITIONS%\Zoom.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowTransition.BlendenUeberblenden\bin\%BUILD%\BlendenUeberblenden.dll" ^
    "%ROOT%\SlideShowTransition.BlendenUeberblenden\bin\%BUILD%\BlendenUeberblenden.pdb" ^
    "%TARGET_TRANSITIONS%\Blenden und Ueberblenden.ssst" ^
    "%TARGET_TRANSITIONS%\Blenden und Ueberblenden.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowTransition.GradientWischen\bin\%BUILD%\SlideShowTransition.GradientWischen.dll" ^
    "%ROOT%\SlideShowTransition.GradientWischen\bin\%BUILD%\SlideShowTransition.GradientWischen.pdb" ^
    "%TARGET_TRANSITIONS%\GradientWischen.ssst" ^
    "%TARGET_TRANSITIONS%\GradientWischen.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowTransition.BrennendesPapier\bin\%BUILD%\SlideShowTransition.BrennendesPapier.dll" ^
    "%ROOT%\SlideShowTransition.BrennendesPapier\bin\%BUILD%\SlideShowTransition.BrennendesPapier.pdb" ^
    "%TARGET_TRANSITIONS%\Brennendes Papier.ssst" ^
    "%TARGET_TRANSITIONS%\Brennendes Papier.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowTransition.VomWindeVerweht\bin\%BUILD%\SlideShowTransition.VomWindeVerweht.dll" ^
    "%ROOT%\SlideShowTransition.VomWindeVerweht\bin\%BUILD%\SlideShowTransition.VomWindeVerweht.pdb" ^
    "%TARGET_TRANSITIONS%\Vom Winde verweht.ssst" ^
    "%TARGET_TRANSITIONS%\Vom Winde verweht.pdb"
if errorlevel 1 goto :Error

:: ============================================================================
:: SHADER
:: ============================================================================

echo.
echo ============================================================================
echo Shader
echo ============================================================================
echo.

call :ExportPlugin ^
    "%ROOT%\SlideShowShader.Invertieren\bin\%BUILD%\SlideShowShader.Invertieren.dll" ^
    "%ROOT%\SlideShowShader.Invertieren\bin\%BUILD%\SlideShowShader.Invertieren.pdb" ^
    "%TARGET_SHADER%\Invertieren.ssss" ^
    "%TARGET_SHADER%\Invertieren.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowShader.Nachtsicht\bin\%BUILD%\SlideShowShader.Nachtsicht.dll" ^
    "%ROOT%\SlideShowShader.Nachtsicht\bin\%BUILD%\SlideShowShader.Nachtsicht.pdb" ^
    "%TARGET_SHADER%\Nachtsicht.ssss" ^
    "%TARGET_SHADER%\Nachtsicht.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowShader.Originalbild\bin\%BUILD%\SlideShowShader.Originalbild.dll" ^
    "%ROOT%\SlideShowShader.Originalbild\bin\%BUILD%\SlideShowShader.Originalbild.pdb" ^
    "%TARGET_SHADER%\Originalbild.ssss" ^
    "%TARGET_SHADER%\Originalbild.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowShader.SchwarzWeiss\bin\%BUILD%\SlideShowShader.SchwarzWeiss.dll" ^
    "%ROOT%\SlideShowShader.SchwarzWeiss\bin\%BUILD%\SlideShowShader.SchwarzWeiss.pdb" ^
    "%TARGET_SHADER%\Schwarz-Weiss.ssss" ^
    "%TARGET_SHADER%\Schwarz-Weiss.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowShader.ToenenFaerben\bin\%BUILD%\SlideShowShader.ToenenFaerben.dll" ^
    "%ROOT%\SlideShowShader.ToenenFaerben\bin\%BUILD%\SlideShowShader.ToenenFaerben.pdb" ^
    "%TARGET_SHADER%\Toenen und Faerben.ssss" ^
    "%TARGET_SHADER%\Toenen und Faerben.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowShader.PixelArt\bin\%BUILD%\SlideShowShader.PixelArt.dll" ^
    "%ROOT%\SlideShowShader.PixelArt\bin\%BUILD%\SlideShowShader.PixelArt.pdb" ^
    "%TARGET_SHADER%\PixelArt.ssss" ^
    "%TARGET_SHADER%\PixelArt.pdb"
if errorlevel 1 goto :Error

call :ExportPlugin ^
    "%ROOT%\SlideShowShader.LUT\bin\%BUILD%\SlideShowShader.LUT.dll" ^
    "%ROOT%\SlideShowShader.LUT\bin\%BUILD%\SlideShowShader.LUT.pdb" ^
    "%TARGET_SHADER%\LUT.ssss" ^
    "%TARGET_SHADER%\LUT.pdb"
if errorlevel 1 goto :Error


:: ============================================================================
:: ORTSDATEN
:: ============================================================================
::
:: Die versionierten Quelldaten liegen unter:
::
::     SlideShowTools\Orte
::
:: Fuer die Laufzeit werden sie nach:
::
::     SlideShowMain\bin\<Build>\Orte
::
:: kopiert.
::
:: Dadurch bleibt bin ein reines Build-/Laufzeitverzeichnis und kann von Git
:: vollstaendig ignoriert werden.
:: ============================================================================

echo.
echo ============================================================================
echo Ortsdaten
echo ============================================================================
echo.

call :CopyDirectory ^
    "%SOURCE_ORTE%" ^
    "%TARGET_ORTE%"
if errorlevel 1 goto :Error


:: ============================================================================
:: D3D - NUR FUER SEHR SEHR HEISSE SEHR SEHR FLACH ZUSAMMENGEPRESSTE HOLZFASERN
:: ============================================================================

:: ============================================================================
:: THIRD-PARTY HELPERS
:: ============================================================================
::
:: Laufzeitabhaengigkeiten externer Bibliotheken, die vom Plugin-Loader
:: nicht als eigene SlideShowSaver-Komponenten behandelt werden.
::
:: Diese DLLs werden neben SlideShowMain.exe bereitgestellt.
:: ============================================================================


:: ============================================================================
:: Direct3D / Vortice / SharpGen
:: ============================================================================

set "D3D_SOURCE=%ROOT%\SlideShowTransition.BrennendesPapier\bin\%BUILD%"

call :CopyRuntime ^
    "%D3D_SOURCE%\Vortice.Direct3D11.dll" ^
    "%MAIN%\Vortice.Direct3D11.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\Vortice.DirectX.dll" ^
    "%MAIN%\Vortice.DirectX.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\Vortice.DXGI.dll" ^
    "%MAIN%\Vortice.DXGI.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\Vortice.Mathematics.dll" ^
    "%MAIN%\Vortice.Mathematics.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\SharpGen.Runtime.dll" ^
    "%MAIN%\SharpGen.Runtime.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\SharpGen.Runtime.COM.dll" ^
    "%MAIN%\SharpGen.Runtime.COM.dll"
if errorlevel 1 goto :Error


:: ============================================================================
:: Tqk WPF / DirectX Interop
:: ============================================================================

call :CopyRuntime ^
    "%D3D_SOURCE%\TqkLibrary.Wpf.Interop.DirectX.dll" ^
    "%MAIN%\TqkLibrary.Wpf.Interop.DirectX.dll"
if errorlevel 1 goto :Error

set "TQK_PACKAGE=%ROOT%\packages\TqkLibrary.Wpf.Interop.DirectX.1.1.17"

call :CopyRuntime ^
    "%TQK_PACKAGE%\runtimes\win-x64\native\TqkLibrary.Wpf.Interop.DirectX.Native.dll" ^
    "%MAIN%\TqkLibrary.Wpf.Interop.DirectX.Native.dll"
if errorlevel 1 goto :Error


:: ============================================================================
:: .NET Runtime Dependencies fuer SharpGen / Vortice
:: ============================================================================

call :CopyRuntime ^
    "%D3D_SOURCE%\System.Memory.dll" ^
    "%MAIN%\System.Memory.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\System.Buffers.dll" ^
    "%MAIN%\System.Buffers.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\System.Runtime.CompilerServices.Unsafe.dll" ^
    "%MAIN%\System.Runtime.CompilerServices.Unsafe.dll"
if errorlevel 1 goto :Error

call :CopyRuntime ^
    "%D3D_SOURCE%\System.Numerics.Vectors.dll" ^
    "%MAIN%\System.Numerics.Vectors.dll"
if errorlevel 1 goto :Error


:: ============================================================================
:: SlideShowSaver Direct3D Interop
:: ============================================================================

call :CopyRuntime ^
    "%ROOT%\SlideShowDirect3DInterop\bin\x64\Debug\SlideShowDirect3DInterop.dll" ^
    "%MAIN%\SlideShowDirect3DInterop.dll"
if errorlevel 1 goto :Error


:: ============================================================================
:: ERFOLG
:: ============================================================================

echo.
echo ============================================================================
echo Export erfolgreich abgeschlossen.
echo ============================================================================
echo.

pause
exit /b 0


:: ============================================================================
:: HILFSPROZEDUREN
:: ============================================================================

:CreateDirectory

if not exist "%~1" (
    echo Erstelle Zielverzeichnis:
    echo   %~1

    mkdir "%~1"

    if errorlevel 1 (
        echo FEHLER: Zielverzeichnis konnte nicht erstellt werden.
        exit /b 1
    )
)

exit /b 0


:ExportPlugin

set "PLUGIN_SOURCE=%~1"
set "PDB_SOURCE=%~2"
set "PLUGIN_TARGET=%~3"
set "PDB_TARGET=%~4"

if not exist "%PLUGIN_SOURCE%" (
    echo.
    echo FEHLER: Plugin wurde nicht gefunden:
    echo   %PLUGIN_SOURCE%
    echo.
    exit /b 1
)

echo Kopiere:
echo   %PLUGIN_SOURCE%
echo nach:
echo   %PLUGIN_TARGET%

copy /Y "%PLUGIN_SOURCE%" "%PLUGIN_TARGET%" >nul

if errorlevel 1 (
    echo.
    echo FEHLER beim Kopieren von:
    echo   %PLUGIN_SOURCE%
    echo.
    exit /b 1
)

if exist "%PDB_SOURCE%" (

    copy /Y "%PDB_SOURCE%" "%PDB_TARGET%" >nul

    if errorlevel 1 (
        echo.
        echo WARNUNG: PDB konnte nicht kopiert werden:
        echo   %PDB_SOURCE%
        echo.
    )

) else (

    echo WARNUNG: PDB wurde nicht gefunden:
    echo   %PDB_SOURCE%
)

echo   OK
echo.

exit /b 0


:CopyRuntime

set "RUNTIME_SOURCE=%~1"
set "RUNTIME_TARGET=%~2"

if not exist "%RUNTIME_SOURCE%" (
    echo.
    echo FEHLER: Runtime-Abhaengigkeit wurde nicht gefunden:
    echo   %RUNTIME_SOURCE%
    echo.
    exit /b 1
)

echo Kopiere Runtime-Abhaengigkeit:
echo   %RUNTIME_SOURCE%
echo nach:
echo   %RUNTIME_TARGET%

copy /Y "%RUNTIME_SOURCE%" "%RUNTIME_TARGET%" >nul

if errorlevel 1 (
    echo.
    echo FEHLER beim Kopieren der Runtime-Abhaengigkeit:
    echo   %RUNTIME_SOURCE%
    echo.
    exit /b 1
)

echo   OK
echo.

exit /b 0


:CopyDirectory

set "DIRECTORY_SOURCE=%~1"
set "DIRECTORY_TARGET=%~2"

if not exist "%DIRECTORY_SOURCE%\" (
    echo.
    echo FEHLER: Quellverzeichnis wurde nicht gefunden:
    echo   %DIRECTORY_SOURCE%
    echo.
    exit /b 1
)

echo Kopiere Verzeichnis:
echo   %DIRECTORY_SOURCE%
echo nach:
echo   %DIRECTORY_TARGET%

if not exist "%DIRECTORY_TARGET%\" (

    mkdir "%DIRECTORY_TARGET%"

    if errorlevel 1 (
        echo.
        echo FEHLER: Zielverzeichnis konnte nicht erstellt werden:
        echo   %DIRECTORY_TARGET%
        echo.
        exit /b 1
    )
)

xcopy "%DIRECTORY_SOURCE%\*" "%DIRECTORY_TARGET%\" /E /I /Y /Q >nul

if errorlevel 1 (
    echo.
    echo FEHLER beim Kopieren des Verzeichnisses:
    echo   %DIRECTORY_SOURCE%
    echo.
    exit /b 1
)

echo   OK
echo.

exit /b 0


:: ============================================================================
:: FEHLER
:: ============================================================================

:Error

echo.
echo ============================================================================
echo EXPORT ABGEBROCHEN!
echo ============================================================================
echo.
echo Mindestens eine erforderliche Datei konnte nicht exportiert werden.
echo Die vorangegangene Fehlermeldung enthaelt den betroffenen Pfad.
echo.

pause
exit /b 1