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
set APP_DIR=%CUR_DIR%..\kinetis_democode\apps\kl_headset\
set FLASHTOOL=%CUR_DIR%flashtool.cmd

if [%1] == [] (
    choice /C AS /N /M "Updating an ADK[A] or SDK[S] board for headset?"
    if ERRORLEVEL 2 (
        set FLASHLIST=%APP_DIR%script\flashlist_release_sdk.yml
        set LAYOUT=%APP_DIR%script\layout_release_sdk.yml
        goto USB_CONFIG_U
    )
    if ERRORLEVEL 1 (
        set FLASHLIST=%APP_DIR%script\flashlist_release_adk_headset.yml
        set LAYOUT=%APP_DIR%script\layout_release_adk_headset.yml
        goto USB_CONFIG_U
    )

    :USB_CONFIG_U
    set /p USBPORT="Enter USB port name of dongle: "
    echo The USB port is %USBPORT%
    goto UPDATE
) else (
    if [%2] == [] (
        echo ERROR: USB port is missing...
        goto HELP_MSG
    )

    if [%1] == [sdk] (
        set FLASHLIST=%APP_DIR%script\flashlist_release_sdk.yml
        set LAYOUT=%APP_DIR%script\layout_release_sdk.yml
        goto USB_CONFIG_C
    )
    if [%1] == [adk] (
        set FLASHLIST=%APP_DIR%script\flashlist_release_adk_headset.yml
        set LAYOUT=%APP_DIR%script\layout_release_adk_headset.yml
        goto USB_CONFIG_C
    )
    echo ERROR: %1 is not a valid board...
    goto HELP_MSG

    :USB_CONFIG_C
    set USBPORT=%2
    echo The USB port is %USBPORT%
)

:UPDATE

if [%3] == [debug] (
    echo.
    echo CUR_DIR '%CUR_DIR%'
    echo APP_DIR '%APP_DIR%'
    echo FLASHTOOL '%FLASHTOOL%'
    echo FLASHLIST '%FLASHLIST%'
    echo LAYOUT '%LAYOUT%'
    echo.
)

cd %CUR_DIR%..
call %FLASHTOOL% --dev %USBPORT% --connection ota --flash-list %FLASHLIST% --layout %LAYOUT% --partition 0 --activate 0 --flash-only nxh_app,rfmac,cf,kl_app -v
exit /b

:HELP_MSG
echo Usage: %~nx0 ^<sdk^|adk^> ^<usb_port^> [debug]
