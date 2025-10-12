@echo off
choice /c YN /m "Are you certain you wish for your data (and the backups) to be deleted? This action cannot be undone."
if errorlevel == 2 exit
if errorlevel == 1 goto Y
:Y
rd /q /s %appdata%\SALG
echo Done.
pause