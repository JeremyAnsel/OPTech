@echo off
setlocal

cd "%~dp0"

For %%a in (
"OPTech\bin\Release\*.dll"
"OPTech\bin\Release\*.exe"
"OPTech\bin\Release\*.bmp"
"OPTech\bin\Release\*.dat"
"OPTech\bin\Release\OPTech.pdb"
) do (
xcopy /s /d "%%~a" dist\
)
