@echo off
setlocal

:: Quellpfade
set "SOURCE1=C:\Users\Arne\source\repos\SlideShowSaver 3.0\Modul Mandelbrot\bin\Debug\Modul Mandelbrot.dll"
set "SOURCE2=C:\Users\Arne\source\repos\SlideShowSaver 3.0\Modul Matrix\bin\Debug\Modul Matrix.dll"
set "SOURCE3=C:\Users\Arne\source\repos\SlideShowSaver 3.0\Modul SlideShowSaver 3.0\bin\Debug\Modul SlideShowSaver 3.0.dll"

:: Zielverzeichnis
set "TARGET=C:\Users\Arne\source\repos\SlideShowSaver 3.0\SlideShowMain\bin\Debug\Module"

:: Sicherstellen, dass das Zielverzeichnis existiert
if not exist "%TARGET%" (
    echo Erstelle Zielverzeichnis: %TARGET%
    mkdir "%TARGET%"
)

:: Kopieren & Umbenennen
echo Kopiere und benenne um...

copy /Y "%SOURCE1%" "%TARGET%\Modul Mandelbrot.sssm"
copy /Y "%SOURCE2%" "%TARGET%\Modul Matrix.sssm"
copy /Y "%SOURCE3%" "%TARGET%\Modul SlideShowSaver 3.0.sssm"

echo Fertig.
pause
