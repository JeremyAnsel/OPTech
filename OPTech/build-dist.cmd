@echo off
setlocal

cd "%~dp0"

For %%a in (
"OPTech\bin\Release\net45\*.dll"
"OPTech\bin\Release\net45\*.exe"
"OPTech\bin\Release\net45\*.exe.config"
"OPTech\bin\Release\net45\*.bmp"
"OPTech\bin\Release\net45\*.dat"
"OPTech\bin\Release\net45\OPTech.pdb"
) do (
xcopy /s /d "%%~a" dist\
)
