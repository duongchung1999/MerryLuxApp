@echo off
SetLocal EnableDelayedExpansion

REM ----------------------------------------------------------------------------
REM Copyright 2020 NXP.
REM
REM This software is owned or controlled by NXP and may only be used strictly
REM in accordance with the applicable license terms. By expressly accepting such
REM terms or by downloading, installing, activating and/or otherwise using the
REM software, you are agreeing that you have read, and that you agree to comply
REM with and are bound by, such license terms. If you do not agree to be bound
REM by the applicable license terms, then you may not retain, install, activate
REM or otherwise use the software.
REM ----------------------------------------------------------------------------

set CUR_DIR=%~dp0
set FLASHTOOL="%CUR_DIR%flashtool.cmd" --work-dir "%CUR_DIR%.."
SET PERSIST_DATAGENTOOL=%CUR_DIR%persist_datagen.cmd
set PERSIST_BIN_FILE=%CUR_DIR%persist_data.bin
set FACT_PERSIST_BIN_FILE=%CUR_DIR%nxh_pers_factory_data.bin
SET DEV_CONF_YAML=%CUR_DIR%nxh_device_info.yml

choice /C DOAMHU /N /M "Flashing Dongle[D], OTA_Dongle[O], Analogue_Dongle[A], Dongle_Mix[M] or Headset[H] or Headset_Usb[U] application?"
if ERRORLEVEL 6 (
    set _app=headset_usb
    goto BOARD_CONFIG
)
if ERRORLEVEL 5 (
    set _app=headset
    goto BOARD_CONFIG
)
if ERRORLEVEL 4 (
    set _app=dongle_mix
    goto BOARD_CONFIG
)
if ERRORLEVEL 3 (
    set _app=dongle_analogue
    goto BOARD_CONFIG
)
if ERRORLEVEL 2 (
    set _app=ota_dongle
    goto BOARD_CONFIG
)
if ERRORLEVEL 1 (
    set _app=dongle
    goto BOARD_CONFIG
)

:BOARD_CONFIG
if not %_app% == dongle_analogue (
    choice /C AS /N /M "Flashing an ADK[A] or SDK[S] board?"
    if ERRORLEVEL 2 (
        set _kl_bsp=sdk
        goto TARGET
    )
    if ERRORLEVEL 1 (
        set _kl_bsp=adk
        goto TARGET
    )
) else (
    echo.
    echo Warning: Analogue_Dongle application can only be flashed to an SDK board.
    echo          Please be sure that an SDK board is being used, not an ADK board.
    echo.
    set _kl_bsp=sdk
)

:TARGET
choice /C RD /N /M "Flashing Release[R] or Debug[D]?"
if ERRORLEVEL 2 (
    set _kl_target=debug
    goto KEEPPAIRING
)
if ERRORLEVEL 1 (
    set _kl_target=release
    goto KEEPPAIRING
)

:KEEPPAIRING
if not %_app% == ota_dongle (
    echo Flash memory will be completely erased before flashing.
    choice /C KW /N /M "Do you like to Keep[K] or Write[W] new nxh persistent data (BD address, ir, er, ...)"
    if ERRORLEVEL 2 (
        choice /C GW /N /M "Do you like to Generate Random device info[G] or Write it from 'nxh_device_info.yml'[W]"
        if ERRORLEVEL 2 (
            set _persist_data_action=write_persist_data
            goto FLASHDEV
        )
        if ERRORLEVEL 1 (
            set _persist_data_action=gen_persist_data
            goto FLASHDEV
        )
    )
    if ERRORLEVEL 1 (
        set _persist_data_action=keep_persist_data
        goto FLASHDEV
    )
) else (
    echo.
    echo Current persistent data will be kept automatically for OTA_Dongle
    set _persist_data_action=keep_persist_data
)

:FLASHDEV
set _kl_app=kl_%_app%
set APP_DIR=%CUR_DIR%..\kinetis_democode\apps\%_kl_app%\
set _flashlist_bsp=%_kl_bsp%
if %_kl_bsp%  == adk (
    set _flashlist_bsp=adk_%_app%
    if %_app% == ota_dongle (
        set _flashlist_bsp=adk_dongle
    )
    if %_app% == dongle_mix (
        set _flashlist_bsp=adk_dongle
    )
    if %_app% == headset_usb (
        set _flashlist_bsp=adk_headset
    )
)

set FLASHLIST=%APP_DIR%script\flashlist_%_kl_target%_%_flashlist_bsp%.yml
set LAYOUT=%APP_DIR%script\layout_%_kl_target%_%_flashlist_bsp%.yml

set _restore=false
set ERRORLEVEL=0

if %_persist_data_action% == gen_persist_data (
    echo Device info is being generated...
    call "!PERSIST_DATAGENTOOL!" --generate-devinfo "!FACT_PERSIST_BIN_FILE!"
)

if %_persist_data_action% == write_persist_data (
    echo Device info is being written from nxh_device_info.yml file
    call "!PERSIST_DATAGENTOOL!" --write-devinfo "!FACT_PERSIST_BIN_FILE!" --devinfo "!DEV_CONF_YAML!"
    if  not exist "!FACT_PERSIST_BIN_FILE!" (
        echo ERROR! Factory Persistent data could not be written into
        echo !FACT_PERSIST_BIN_FILE!
        echo.
        exit /b 1
    )
)

if %_persist_data_action% == keep_persist_data (
    echo Current persistent data in the device is being backed up
    call !FLASHTOOL! --dev kl --connection usb --dump-to-file "!PERSIST_BIN_FILE!" --partition nxh_pers_data
    if  not exist "!PERSIST_BIN_FILE!" (
        echo ERROR! Persistent data could not be dumped into
        echo "!PERSIST_BIN_FILE!"
        echo Partition table might be outdated, or flash is empty
        echo.
        exit /b 1
    )
    call !FLASHTOOL! --dev kl --connection usb --dump-to-file "!FACT_PERSIST_BIN_FILE!" --partition nxh_pers_factry
    if  not exist "!FACT_PERSIST_BIN_FILE!" (
        echo ERROR! Factory Persistent data could not be dumped into
        echo "!FACT_PERSIST_BIN_FILE!"
        echo Partition table might be outdated, or flash is empty
        echo.
        exit /b 1
    )
    set _restore=true
)

if not %ERRORLEVEL% == 0 (
    echo Error! Terminating...
    goto FINISH
)

call !FLASHTOOL! --dev kl --connection usb --erase-chip --flash-list "!FLASHLIST!" --layout "!LAYOUT!" -v

if not %ERRORLEVEL% == 0 (
    echo Error! Terminating...
    goto FINISH
)

if %_restore% == true (
    call !FLASHTOOL! --dev kl --connection usb --restore-from-file "!PERSIST_BIN_FILE!" --partition nxh_pers_data
    del "!PERSIST_BIN_FILE!"
)

if not %ERRORLEVEL% == 0 (
    echo Error! Terminating...
    goto FINISH
)

call !FLASHTOOL! --dev kl --connection usb --restore-from-file "!FACT_PERSIST_BIN_FILE!" --partition nxh_pers_factry
del "!FACT_PERSIST_BIN_FILE!"
if not %ERRORLEVEL% == 0 (
    echo Error! Terminating...
    goto FINISH
)
call !FLASHTOOL! --dev kl --connection usb --reset

:FINISH
exit /b %ERRORLEVEL%
