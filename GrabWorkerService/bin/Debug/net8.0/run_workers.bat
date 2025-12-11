@echo off
setlocal ENABLEDELAYEDEXPANSION

echo ==========================================
echo   AOI Platform - Worker Auto Launcher
echo ==========================================
echo.

REM ====== 設定參數 ======
set GROUP=1
set TOP_COUNT=2
set BTM_COUNT=2
set INTERVAL=150

REM ====== 自動定位 GrabWorkerService.exe ======

echo 尋找 GrabWorkerService.exe ...

set EXE_PATH=

REM 1. 優先找 Release
for /r %%f in (GrabWorkerService.exe) do (
    echo 找到：%%f
    set EXE_PATH=%%f
    goto FOUND_EXE
)

:FOUND_EXE

if "%EXE_PATH%"=="" (
    echo [錯誤] 找不到 GrabWorkerService.exe
    echo 請確認你已經建置專案（Build）
    echo.
    pause
    exit /b
)

echo 使用執行檔：
echo   %EXE_PATH%
echo.

REM ====== 啟動 Top Workers ======
echo 啟動 Top Workers ...
for /L %%i in (1,1,%TOP_COUNT%) do (
    start "GrabWorker-Top%%i" "%EXE_PATH%" ^
        --GrabWorker:GroupId=%GROUP% ^
        --GrabWorker:Side=Top ^
        --GrabWorker:WorkerId=%%i ^
        --GrabWorker:MockTriggerIntervalMs=%INTERVAL%
)

REM ====== 啟動 Bottom Workers ======
echo 啟動 Bottom Workers ...
for /L %%i in (1,1,%BTM_COUNT%) do (
    start "GrabWorker-Bottom%%i" "%EXE_PATH%" ^
        --GrabWorker:GroupId=%GROUP% ^
        --GrabWorker:Side=Bottom ^
        --GrabWorker:WorkerId=%%i ^
        --GrabWorker:MockTriggerIntervalMs=%INTERVAL%
)

echo.
echo =============================================
echo Worker 啟動完成！
echo Top:    %TOP_COUNT%
echo Bottom: %BTM_COUNT%
echo =============================================
echo.
pause
