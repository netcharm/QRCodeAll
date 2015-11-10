@echo off

XCOPY /Y *.cs src

set SRC=%~dp0%src
set DST=..\..\Build\QrCodeAnyPlugin
set KEEPASS=%DST%\KeePass.exe

%KEEPASS% --plgx-create %SRC%
move /Y %SRC%.plgx %DST%\QrCodeAny.plgx
