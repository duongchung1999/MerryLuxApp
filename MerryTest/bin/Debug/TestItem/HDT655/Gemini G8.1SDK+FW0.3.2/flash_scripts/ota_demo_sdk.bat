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

echo ######## Generating Devinfo and bonding information
call persist_datagen.cmd --generate-devinfo headset-devinfo.bin
call persist_datagen.cmd --generate-devinfo dongle-devinfo.bin
echo.
echo.
echo ######## flashing the dongle ...
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --erase-chip
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --flash-list kinetis_democode\apps\kl_dongle\script\flashlist_release_sdk.yml --layout kinetis_democode\apps\kl_dongle\script\layout_release_sdk.yml -v
call flashtool.cmd --work-dir .\ --dev kl --connection usb --partition nxh_pers_factry --restore-from-file dongle-devinfo.bin
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --reset
rem *** HACK for win 7 / 10 compat: Sleep for 5 seconds ***
ping -n 5 127.0.0.1 > nul

echo.
echo.
echo ######## flashing the headset ...
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --erase-chip
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --flash-list kinetis_democode\apps\kl_headset\script\flashlist_release_sdk.yml --layout kinetis_democode\apps\kl_headset\script\layout_release_sdk.yml -v
call flashtool.cmd --work-dir .\ --dev kl --connection usb --partition nxh_pers_factry --restore-from-file headset-devinfo.bin
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --reset

echo.
echo.
echo ######## devices now pairing (leds should blink then stop blinking on both devices) ...
rem *** HACK for win 7 / 10 compat: Sleep for 10 seconds ***
ping -n 10 127.0.0.1 > nul

echo.
echo.
echo ######## flashing the OTA dongle ...
call flashtool.cmd --work-dir .\ --dev kl --connection usb --partition nxh_pers_data --dump-to-file dongle_nxh_pers_data.bin
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --flash-list kinetis_democode\apps\kl_ota_dongle\script\flashlist_release_sdk.yml --layout kinetis_democode\apps\kl_ota_dongle\script\layout_release_sdk.yml -v
call flashtool.cmd --work-dir .\ --dev kl --connection usb --partition nxh_pers_data --restore-from-file dongle_nxh_pers_data.bin
rm dongle_nxh_pers_data.bin
call flashtool.cmd --work-dir ..\ --dev kl --connection usb --reset

echo.
echo.
set /p input="######## please reset both boards (main led should light up and not blink); then press any key to continue ..."
echo.
set /p _ota_dev="######## OTA Dongle should now be connected as a serial device, please enter the COM port name: "
echo.

rem *** HACK for win 7 / 10 compat: Sleep for 3 seconds ***
ping -n 3 127.0.0.1 > nul
echo.
echo ######## OTA updating headset ...
call flashtool.cmd --work-dir ..\ --dev %_ota_dev% --connection ota --flash-list kinetis_democode\apps\kl_headset\script\flashlist_release_sdk.yml --layout kinetis_democode\apps\kl_headset\script\layout_release_sdk.yml --partition 0 --flash-only nxh_app,rfmac,cf,kl_app -v --activate 0
