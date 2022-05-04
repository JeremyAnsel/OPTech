@echo off
setlocal

cd "%~dp0"

For %%a in (
"OPTech\bin\Release\net48\*.dll"
"OPTech\bin\Release\net48\*.exe"
"OPTech\bin\Release\net48\*.exe.config"
"OPTech\bin\Release\net48\*.bmp"
"OPTech\bin\Release\net48\*.dat"
"OPTech\bin\Release\net48\OPTech.pdb"
) do (
xcopy /s /d "%%~a" dist\
)
