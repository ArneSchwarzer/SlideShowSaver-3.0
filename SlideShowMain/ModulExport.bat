@echo off
setlocal


:: === Module ===
:: Quellpfade
set "SOURCE1=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowModul.Mandelbrot\bin\Debug\SlideShowModul.Mandelbrot.dll"
set "SOURCE2=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowModul.Matrix\bin\Debug\SlideShowModul.Matrix.dll"
set "SOURCE3=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowModulSlideShowSaver 3.0\bin\Debug\SlideShowModul.SlideShowSaver 3.0.dll"

:: Zielverzeichnis
set "TARGET=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Module"

:: Sicherstellen, dass das Zielverzeichnis existiert
if not exist "%TARGET%" (
    echo Erstelle Zielverzeichnis: %TARGET%
    mkdir "%TARGET%"
)

:: Kopieren & Umbenennen
echo Kopiere Module und benenne um...
echo

copy /Y "%SOURCE1%" "%TARGET%\Mandelbrot.sssm"
copy /Y "%SOURCE2%" "%TARGET%\Matrix.sssm"
copy /Y "%SOURCE3%" "%TARGET%\SlideShowSaver 3.0.sssm"

echo Module kopiert.
echo
echo

:: === Transitionen ===
:: Quellpfade
set "SOURCE1=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.Cut\bin\Debug\SlideShowTransition.Cut.dll"
set "SOURCE2=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowTransition.SchiebenWischen\bin\Debug\SlideShowTransition.SchiebenWischen.dll"

:: Zielverzeichnis
set "TARGET=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Transitions"

:: Sicherstellen, dass das Zielverzeichnis existiert
if not exist "%TARGET%" (
    echo Erstelle Zielverzeichnis: %TARGET%
    mkdir "%TARGET%"
)

:: Kopieren & Umbenennen
echo Kopiere Transitionen und benenne um...
echo

copy /Y "%SOURCE1%" "%TARGET%\Direkter Uebergang.ssst"
copy /Y "%SOURCE2%" "%TARGET%\Schieben und Wischen.ssst"

echo Transitionen kopiert.
echo
echo

:: === Shader ===
:: Quellpfade
set "SOURCE1=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Originalbild\bin\Debug\SlideShowShader.Originalbild.dll"
set "SOURCE2=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.Nachtsicht\bin\Debug\SlideShowShader.Nachtsicht.dll"
set "SOURCE3=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.SchwarzWeiss\bin\Debug\SlideShowShader.SchwarzWeiss.dll"
set "SOURCE4=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowShader.ToenenFaerben\bin\Debug\SlideShowShader.ToenenFaerben.dll"

:: Zielverzeichnis
set "TARGET=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Shader"

:: Sicherstellen, dass das Zielverzeichnis existiert
if not exist "%TARGET%" (
    echo Erstelle Zielverzeichnis: %TARGET%
    mkdir "%TARGET%"
)

:: Kopieren & Umbenennen
echo Kopiere Shader und benenne um...
echo

copy /Y "%SOURCE1%" "%TARGET%\Originalbild.ssss"
copy /Y "%SOURCE2%" "%TARGET%\Nachtsicht.ssss"
copy /Y "%SOURCE3%" "%TARGET%\Schwarz-Weiss.ssss"
copy /Y "%SOURCE4%" "%TARGET%\Toenen und Faerben.ssss"

echo Shader kopiert.
echo
echo Fertig.
pause
