@echo off
setlocal


:: === Module ===
:: Quellpfade
set "SOURCE1=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowModul.Mandelbrot\bin\Debug\SlideShowModul.Mandelbrot.dll"
set "SOURCE2=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowModul.Mandelbrot\bin\Debug\SlideShowModul.Mandelbrot.pdb"
set "SOURCE3=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowModul.Matrix\bin\Debug\SlideShowModul.Matrix.dll"
set "SOURCE4=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowModul.Matrix\bin\Debug\SlideShowModul.Matrix.pdb"
set "SOURCE5=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowWPFModul.SlideShowSaver 3.0\bin\Debug\SlideShowWPFModul.SlideShowSaver 3.0.dll"
set "SOURCE6=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowWPFModul.SlideShowSaver 3.0\bin\Debug\SlideShowWPFModul.SlideShowSaver 3.0.pdb"

:: Zielverzeichnis
set "TARGET=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Module"

:: Sicherstellen, dass das Zielverzeichnis existiert
if not exist "%TARGET%" (
    echo Erstelle Zielverzeichnis: %TARGET%
    mkdir "%TARGET%"
)

:: Kopieren & Umbenennen
echo Kopiere Module und benenne um...
echo.

copy /Y "%SOURCE1%" "%TARGET%\Mandelbrot.sssm"
copy /Y "%SOURCE2%" "%TARGET%\Mandelbrot.pdb"
copy /Y "%SOURCE3%" "%TARGET%\Matrix.sssm"
copy /Y "%SOURCE4%" "%TARGET%\Matrix.pdb"
copy /Y "%SOURCE5%" "%TARGET%\SlideShowSaver 3.0.sssm"
copy /Y "%SOURCE6%" "%TARGET%\SlideShowSaver 3.0.pdb"


echo Module kopiert.
echo.
echo.

:: === Transitionen ===
:: Quellpfade
set "SOURCE1=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.Cut\bin\Debug\SlideShowTransition.Cut.dll"
set "SOURCE2=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.Cut\bin\Debug\SlideShowTransition.Cut.pdb"
set "SOURCE3=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowWPFTransition.SchiebenWischen\bin\Debug\SlideShowWPFTransition.SchiebenWischen.dll"
set "SOURCE4=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowWPFTransition.SchiebenWischen\bin\Debug\SlideShowWPFTransition.SchiebenWischen.pdb"
set "SOURCE5=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowWPFTransition.Zoom\bin\Debug\SlideShowWPFTransition.Zoom.dll"
set "SOURCE6=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowWPFTransition.Zoom\bin\Debug\SlideShowWPFTransition.Zoom.pdb"
set "SOURCE7=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.BlendenUeberblenden\bin\Debug\BlendenUeberblenden.dll"
set "SOURCE8=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.BlendenUeberblenden\bin\Debug\BlendenUeberblenden.pdb"
set "SOURCE9=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.GradientWischen\bin\Debug\SlideShowTransition.GradientWischen.dll"
set "SOURCE10=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.GradientWischen\bin\Debug\SlideShowTransition.GradientWischen.pdb"

:: Zielverzeichnis
set "TARGET=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Transitions"

:: Sicherstellen, dass das Zielverzeichnis existiert
if not exist "%TARGET%" (
    echo Erstelle Zielverzeichnis: %TARGET%
    mkdir "%TARGET%"
)

:: Kopieren & Umbenennen
echo Kopiere Transitionen und benenne um...
echo.

copy /Y "%SOURCE1%" "%TARGET%\Direkter Uebergang.ssst"
copy /Y "%SOURCE2%" "%TARGET%\Direkter Uebergang.pdb"
copy /Y "%SOURCE3%" "%TARGET%\Schieben und Wischen.ssst"
copy /Y "%SOURCE4%" "%TARGET%\Schieben und Wischen.pdb"
copy /Y "%SOURCE5%" "%TARGET%\Zoom.ssst"
copy /Y "%SOURCE6%" "%TARGET%\Zoom.pdb"
copy /Y "%SOURCE7%" "%TARGET%\Blenden und Ueberblenden.ssst"
copy /Y "%SOURCE8%" "%TARGET%\Blenden und Ueberblenden.pdb"
copy /Y "%SOURCE9%" "%TARGET%\GradientWischen.ssst"
copy /Y "%SOURCE10%" "%TARGET%\GradientWischen.pdb"

echo Transitionen kopiert.
echo.
echo.

:: === Shader ===
:: Quellpfade

set "SOURCE1=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Invertieren\bin\Debug\SlideShowShader.Invertieren.dll"
set "SOURCE2=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Invertieren\bin\Debug\SlideShowShader.Invertieren.pdb"
set "SOURCE3=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Nachtsicht\bin\Debug\SlideShowShader.Nachtsicht.dll"
set "SOURCE4=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Nachtsicht\bin\Debug\SlideShowShader.Nachtsicht.pdb"
set "SOURCE5=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Originalbild\bin\Debug\SlideShowShader.Originalbild.dll"
set "SOURCE6=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Originalbild\bin\Debug\SlideShowShader.Originalbild.pdb"
set "SOURCE7=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.SchwarzWeiss\bin\Debug\SlideShowShader.SchwarzWeiss.dll"
set "SOURCE8=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.SchwarzWeiss\bin\Debug\SlideShowShader.SchwarzWeiss.pdb"
set "SOURCE9=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.ToenenFaerben\bin\Debug\SlideShowShader.ToenenFaerben.dll"
set "SOURCE10=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.ToenenFaerben\bin\Debug\SlideShowShader.ToenenFaerben.pdb"

:: Zielverzeichnis
set "TARGET=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Shader"

:: Sicherstellen, dass das Zielverzeichnis existiert
if not exist "%TARGET%" (
    echo Erstelle Zielverzeichnis: %TARGET%
    mkdir "%TARGET%"
)

:: Kopieren & Umbenennen
echo Kopiere Shader und benenne um...
echo.

copy /Y "%SOURCE1%" "%TARGET%\Invertieren.ssss"
copy /Y "%SOURCE2%" "%TARGET%\Invertieren.pdb"
copy /Y "%SOURCE3%" "%TARGET%\Nachtsicht.ssss"
copy /Y "%SOURCE4%" "%TARGET%\Nachtsicht.pdb"
copy /Y "%SOURCE5%" "%TARGET%\Originalbild.ssss"
copy /Y "%SOURCE6%" "%TARGET%\Originalbild.pdb"
copy /Y "%SOURCE7%" "%TARGET%\Schwarz-Weiss.ssss"
copy /Y "%SOURCE8%" "%TARGET%\Schwarz-Weiss.pdb"
copy /Y "%SOURCE9%" "%TARGET%\Toenen und Faerben.ssss"
copy /Y "%SOURCE10%" "%TARGET%\Toenen und Faerben.pdb"

echo Shader kopiert.
echo.
echo Fertig.
pause
