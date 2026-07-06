@echo off

set FXC="C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\fxc.exe"

%FXC% /T ps_3_0 /E main /Fo Mandelbrot.ps Mandelbrot.fx

pause