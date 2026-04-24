@echo off

set PROJECT_PATH=%~dp0
set KEEPASS_PATH="%PROJECT_PATH%libs\KeePass.exe"
set SOURCE_PATH="%PROJECT_PATH%QuickConnectPlugin"
set PLGX_NAME=NeoQuickConnectPlugin.plgx

cd /d "%PROJECT_PATH%"

IF NOT EXIST build (
	mkdir build
)

IF EXIST .\build\QuickConnectPlugin.plgx (
	del /Q .\build\QuickConnectPlugin.plgx
)

IF EXIST .\build\%PLGX_NAME% (
	del /Q .\build\%PLGX_NAME%
)

xcopy /Y .\QuickConnectPlugin\bin\Debug\QuickConnectPlugin.dll .\build\
xcopy /Y .\QuickConnectPlugin\bin\Debug\QuickConnectPlugin.pdb .\build\

echo Cleaning project directory...
IF EXIST .\QuickConnectPlugin\bin\ (
	rmdir /S /Q .\QuickConnectPlugin\bin\
)

IF EXIST .\QuickConnectPlugin\obj\ (
	rmdir /S /Q .\QuickConnectPlugin\obj\
)

echo Building PLGX file...

copy /Y .\QuickConnectPlugin\Info.cs .\QuickConnectPlugin\Info.cs.bak
xcopy /Y .\Info.cs .\QuickConnectPlugin\

REM KeePass PLGX prerequisites use CLR 4.x notation. Newer .NET Framework 4.x
REM releases, including 4.8, are in-place updates and should not be written here.
%KEEPASS_PATH% --plgx-prereq-kp:2.52 --plgx-prereq-net:4.0 --plgx-create %SOURCE_PATH%

echo Moving PLGX file to build directory...
move /Y .\QuickConnectPlugin.plgx .\build\%PLGX_NAME%

echo Cleaning PLGX build directory....

move /Y .\QuickConnectPlugin\Info.cs.bak .\QuickConnectPlugin\Info.cs

echo Done.
pause
