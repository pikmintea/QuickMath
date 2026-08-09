
@echo off
setlocal EnableExtensions EnableDelayedExpansion

REM ============================================================
REM QuickMath - .NET 10 Multi-Platform Build
REM
REM Non-destructive:
REM   - Does NOT delete source files
REM   - Does NOT clean the solution
REM   - Does NOT run "dotnet clean"
REM   - Failed platforms do not stop other builds
REM
REM Output:
REM   build\windows\x64
REM   build\windows\x86
REM   build\android\x64
REM   build\android\x86
REM   build\linux\x64
REM   build\macos\x64
REM   build\macos\arm64
REM ============================================================
cls
title QuickMath .NET 10 Multi-Platform Build

cd /d "%~dp0"

set "ROOT=%CD%"
set "BUILD=%ROOT%\build"
set "LOG=%BUILD%\build.log"
cls
echo.
echo ============================================================
echo                 QuickMath Build System
echo ============================================================
echo.
echo Root:
echo %ROOT%
echo.
echo Build output:
echo %BUILD%
echo.
cls
REM ------------------------------------------------------------
REM Check .NET
REM ------------------------------------------------------------

where dotnet >nul 2>&1
if errorlevel 1 (
    echo ERROR: dotnet was not found in PATH.
    echo Install .NET 10 SDK and try again.
    pause
    exit /b 1
)

echo .NET:
dotnet --version
echo.
cls
REM ------------------------------------------------------------
REM Locate projects
REM ------------------------------------------------------------

set "APP_PROJECT="
set "WEB_PROJECT="

if exist "%ROOT%\QuickMath\QuickMath.csproj" (
    set "APP_PROJECT=%ROOT%\QuickMath\QuickMath.csproj"
)

if exist "%ROOT%\QuickMath.Web\QuickMath.Web.csproj" (
    set "WEB_PROJECT=%ROOT%\QuickMath.Web\QuickMath.Web.csproj"
)

REM If the exact filenames are different, find the first csproj
REM in the expected folders.

if not defined APP_PROJECT (
    for /f "delims=" %%F in ('dir /b /s "%ROOT%\QuickMath\*.csproj" 2^>nul') do (
        if not defined APP_PROJECT set "APP_PROJECT=%%F"
    )
)

if not defined WEB_PROJECT (
    for /f "delims=" %%F in ('dir /b /s "%ROOT%\QuickMath.Web\*.csproj" 2^>nul') do (
        if not defined WEB_PROJECT set "WEB_PROJECT=%%F"
    )
)
cls
echo ------------------------------------------------------------
echo Projects
echo ------------------------------------------------------------

if defined APP_PROJECT (
    echo MAUI/App:
    echo   %APP_PROJECT%
) else (
    echo WARNING: QuickMath MAUI project not found.
)

if defined WEB_PROJECT (
    echo Web:
    echo   %WEB_PROJECT%
) else (
    echo WARNING: QuickMath.Web project not found.
)

echo.
cls
REM ------------------------------------------------------------
REM Create output directories
REM ------------------------------------------------------------

if not exist "%BUILD%" mkdir "%BUILD%"

if not exist "%BUILD%\windows\x64" mkdir "%BUILD%\windows\x64"
if not exist "%BUILD%\windows\x86" mkdir "%BUILD%\windows\x86"

if not exist "%BUILD%\android\x64" mkdir "%BUILD%\android\x64"
if not exist "%BUILD%\android\x86" mkdir "%BUILD%\android\x86"

if not exist "%BUILD%\linux\x64" mkdir "%BUILD%\linux\x64"

if not exist "%BUILD%\macos\x64" mkdir "%BUILD%\macos\x64"
if not exist "%BUILD%\macos\arm64" mkdir "%BUILD%\macos\arm64"

echo Build started %DATE% %TIME% > "%LOG%"
echo. >> "%LOG%"
cls
REM ------------------------------------------------------------
REM Restore
REM ------------------------------------------------------------
cls
echo ============================================================
echo RESTORE
echo ============================================================
echo.

dotnet restore "%ROOT%\QuickMath.slnx" >> "%LOG%" 2>&1

if errorlevel 1 (
    echo WARNING: Restore reported an error.
    echo Check:
    echo %LOG%
    echo.
    echo Continuing anyway...
) else (
    echo Restore OK.
)

echo. >> "%LOG%"
cls
REM ============================================================
REM WINDOWS
REM ============================================================
cls
echo.
echo ============================================================
echo WINDOWS x64
echo ============================================================
echo.

if defined APP_PROJECT (
    call :BuildApp ^
        "WINDOWS x64" ^
        "%APP_PROJECT%" ^
        "net10.0-windows10.0.19041.0" ^
        "win-x64" ^
        "%BUILD%\windows\x64"
) else (
    echo SKIPPED - MAUI project not found.
)
cls
echo.
echo ============================================================
echo WINDOWS x86
echo ============================================================
echo.

if defined APP_PROJECT (
    call :BuildApp ^
        "WINDOWS x86" ^
        "%APP_PROJECT%" ^
        "net10.0-windows10.0.19041.0" ^
        "win-x86" ^
        "%BUILD%\windows\x86"
) else (
    echo SKIPPED - MAUI project not found.
)
cls
REM ============================================================
REM ANDROID
REM ============================================================
cls
echo.
echo ============================================================
echo ANDROID x64
echo ============================================================
echo.

if defined APP_PROJECT (
    call :BuildApp ^
        "ANDROID x64" ^
        "%APP_PROJECT%" ^
        "net10.0-android" ^
        "android-x64" ^
        "%BUILD%\android\x64"
) else (
    echo SKIPPED - MAUI project not found.
)
cls
echo.
echo ============================================================
echo ANDROID x86
echo ============================================================
echo.

if defined APP_PROJECT (
    call :BuildApp ^
        "ANDROID x86" ^
        "%APP_PROJECT%" ^
        "net10.0-android" ^
        "android-x86" ^
        "%BUILD%\android\x86"
) else (
    echo SKIPPED - MAUI project not found.
)
cls
REM ============================================================
REM LINUX
REM ============================================================

echo.
echo ============================================================
echo LINUX x64
echo ============================================================
echo.

if defined WEB_PROJECT (
    call :BuildWeb ^
        "LINUX x64" ^
        "%WEB_PROJECT%" ^
        "linux-x64" ^
        "%BUILD%\linux\x64"
) else (
    echo SKIPPED - Web project not found.
)
cls
REM ============================================================
REM MACOS
REM ============================================================
cls
echo.
echo ============================================================
echo MACOS x64
echo ============================================================
echo.

if defined APP_PROJECT (
    call :BuildApp ^
        "MACOS x64" ^
        "%APP_PROJECT%" ^
        "net10.0-maccatalyst" ^
        "maccatalyst-x64" ^
        "%BUILD%\macos\x64"
) else (
    echo SKIPPED - MAUI project not found.
)
cls
echo.
echo ============================================================
echo MACOS ARM64
echo ============================================================
echo.

if defined APP_PROJECT (
    call :BuildApp ^
        "MACOS ARM64" ^
        "%APP_PROJECT%" ^
        "net10.0-maccatalyst" ^
        "maccatalyst-arm64" ^
        "%BUILD%\macos\arm64"
) else (
    echo SKIPPED - MAUI project not found.
)
cls
REM ============================================================
REM DONE
REM ============================================================
cls
echo.
echo ============================================================
echo                    BUILD FINISHED
echo ============================================================
echo.

echo Output:
echo.
echo   %BUILD%\windows\x64
echo   %BUILD%\windows\x86
echo   %BUILD%\android\x64
echo   %BUILD%\android\x86
echo   %BUILD%\linux\x64
echo   %BUILD%\macos\x64
echo   %BUILD%\macos\arm64
echo.

echo Full build log:
echo   %LOG%
echo.

echo Finished %DATE% %TIME% >> "%LOG%"

pause
exit /b 0

cls
REM ============================================================
REM Build MAUI application
REM ============================================================

:BuildApp

set "NAME=%~1"
set "PROJECT=%~2"
set "TFM=%~3"
set "RID=%~4"
set "OUTPUT=%~5"
cls
echo.
echo ------------------------------------------------------------
echo Building %NAME%
echo ------------------------------------------------------------
echo Project: %PROJECT%
echo TFM:     %TFM%
echo RID:     %RID%
echo Output:  %OUTPUT%
echo.

echo ============================================================ >> "%LOG%"
echo %NAME% >> "%LOG%"
echo Project: %PROJECT% >> "%LOG%"
echo TFM: %TFM% >> "%LOG%"
echo RID: %RID% >> "%LOG%"
echo Output: %OUTPUT% >> "%LOG%"
echo ============================================================ >> "%LOG%"

if not exist "%OUTPUT%" mkdir "%OUTPUT%"

dotnet publish "%PROJECT%" ^
    -c Release ^
    -f "%TFM%" ^
    -r "%RID%" ^
    --self-contained true ^
    -p:SelfContained=true ^
    -p:PublishReadyToRun=true ^
    -p:PublishSingleFile=false ^
    -p:DebugType=None ^
    -p:DebugSymbols=false ^
    -o "%OUTPUT%" >> "%LOG%" 2>&1

if errorlevel 1 (
    echo.
    echo [FAILED] %NAME%
    echo See build.log for details.
    echo.
    echo [FAILED] %NAME% >> "%LOG%"
) else (
    echo.
    echo [OK] %NAME%
    echo [OK] %NAME% >> "%LOG%"
)

exit /b 0
cls

REM ============================================================
REM Build Blazor Web App
REM ============================================================

:BuildWeb

set "NAME=%~1"
set "PROJECT=%~2"
set "RID=%~3"
set "OUTPUT=%~4"
cls
echo.
echo ------------------------------------------------------------
echo Building %NAME%
echo ------------------------------------------------------------
echo Project: %PROJECT%
echo RID:     %RID%
echo Output:  %OUTPUT%
echo.

echo ============================================================ >> "%LOG%"
echo %NAME% >> "%LOG%"
echo Project: %PROJECT% >> "%LOG%"
echo RID: %RID% >> "%LOG%"
echo Output: %OUTPUT% >> "%LOG%"
echo ============================================================ >> "%LOG%"

if not exist "%OUTPUT%" mkdir "%OUTPUT%"

dotnet publish "%PROJECT%" ^
    -c Release ^
    -r "%RID%" ^
    --self-contained true ^
    -p:SelfContained=true ^
    -p:PublishReadyToRun=true ^
    -p:PublishSingleFile=false ^
    -p:DebugType=None ^
    -p:DebugSymbols=false ^
    -o "%OUTPUT%" >> "%LOG%" 2>&1

if errorlevel 1 (
    echo.
    echo [FAILED] %NAME%
    echo See build.log for details.
    echo.
    echo [FAILED] %NAME% >> "%LOG%"
) else (
    echo.
    echo [OK] %NAME%
    echo [OK] %NAME% >> "%LOG%"
)
