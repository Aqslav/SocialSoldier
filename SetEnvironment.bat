@echo off
for /f "usebackq tokens=1,* delims==" %%A in ("%~dp0.env") do (
    if not "%%A"=="" if not "%%A:~0,1%%"=="#" setx "%%A" "%%B"
)
pause