#!/usr/bin/env bash

# ----------------------------------------------------------------------------
# Copyright 2020 NXP.
#
# This software is owned or controlled by NXP and may only be used strictly
# in accordance with the applicable license terms. By expressly accepting such
# terms or by downloading, installing, activating and/or otherwise using the
# software, you are agreeing that you have read, and that you agree to comply
# with and are bound by, such license terms. If you do not agree to be bound
# by the applicable license terms, then you may not retain, install, activate
# or otherwise use the software.
# ----------------------------------------------------------------------------

set -e

echo ">>>>>>>> Generating Devinfo and bonding information..."
./persist_datagen.cmd --generate-devinfo headset-devinfo.bin
./persist_datagen.cmd --generate-devinfo dongle-devinfo.bin

echo ">>>>>>>> flashing the dongle ..."
./flashtool.cmd --work-dir ../ --dev kl --connection usb --erase-chip
./flashtool.cmd --work-dir ../ --dev kl --connection usb --flash-list kinetis_democode/apps/kl_dongle/script/flashlist_release_sdk.yml --layout kinetis_democode/apps/kl_dongle/script/layout_release_sdk.yml -v
./flashtool.cmd --work-dir ./ --dev kl --connection usb --partition nxh_pers_factry --restore-from-file dongle-devinfo.bin
./flashtool.cmd --work-dir ../ --dev kl --connection usb --reset
sleep 5

echo
echo
echo ">>>>>>>> flashing the headset ..."
./flashtool.cmd --work-dir ../ --dev kl --connection usb --erase-chip
./flashtool.cmd --work-dir ../ --dev kl --connection usb --flash-list kinetis_democode/apps/kl_headset/script/flashlist_release_sdk.yml --layout kinetis_democode/apps/kl_headset/script/layout_release_sdk.yml -v
./flashtool.cmd --work-dir ./ --dev kl --connection usb --partition nxh_pers_factry --restore-from-file headset-devinfo.bin
./flashtool.cmd --work-dir ../ --dev kl --connection usb --reset

echo
echo
echo ">>>>>>>> devices now pairing (leds should blink then stop blinking on both devices) ..."
sleep 10

echo
echo
echo ">>>>>>>> flashing the OTA dongle ..."
./flashtool.cmd --work-dir ./ --dev kl --connection usb --partition nxh_pers_data --dump-to-file dongle_nxh_pers_data.bin
./flashtool.cmd --work-dir ../ --dev kl --connection usb --flash-list kinetis_democode/apps/kl_ota_dongle/script/flashlist_release_sdk.yml --layout kinetis_democode/apps/kl_ota_dongle/script/layout_release_sdk.yml -v
./flashtool.cmd --work-dir ./ --dev kl --connection usb --partition nxh_pers_data --restore-from-file dongle_nxh_pers_data.bin
rm dongle_nxh_pers_data.bin
./flashtool.cmd --work-dir ../ --dev kl --connection usb --reset

echo
echo
read -r -p ">>>>>>>> please reset both boards (main led should light up and not blink); then press any key to continue ..."
echo
read -r -p ">>>>>>>> OTA Dongle should now be connected as a serial device, please enter its name (COMxx on Windows, /dev/ttyxxx on Mac/Linux): " _ota_dev;
echo

if [ ! -w "${_ota_dev}" ]; then
    echo "ERROR: not a valid/readable port: ${_ota_dev}"
    exit 1
fi

sleep 3
echo
echo ">>>>>>>> OTA updating headset ..."
./flashtool.cmd --work-dir ../ --dev "${_ota_dev}" --connection ota --flash-list kinetis_democode/apps/kl_headset/script/flashlist_release_sdk.yml --layout kinetis_democode/apps/kl_headset/script/layout_release_sdk.yml --partition 0 --flash-only nxh_app,rfmac,cf,kl_app -v --activate 0
