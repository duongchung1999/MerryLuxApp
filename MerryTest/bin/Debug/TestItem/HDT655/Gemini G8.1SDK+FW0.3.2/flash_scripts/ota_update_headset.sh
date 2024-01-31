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

HELP_MSG() {
    echo "Usage: $0 <sdk|adk> <usb_port> [debug]"
    exit 1
}

CUR_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"/
APP_DIR=${CUR_DIR}../kinetis_democode/apps/kl_headset/
FLASHTOOL=${CUR_DIR}flashtool.cmd

if [ $# -eq 0 ]; then
    read -r -n 1 -p "Updating an ADK[A] or SDK[S] board for headset?" option;
    case ${option} in
        s|S)
            FLASHLIST=${APP_DIR}script/flashlist_release_sdk.yml;
            LAYOUT=${APP_DIR}script/layout_release_sdk.yml;;
        a|A)
            FLASHLIST=${APP_DIR}script/flashlist_release_adk_headset.yml;
            LAYOUT=${APP_DIR}script/layout_release_adk_headset.yml;;
        *) exit 1;;
    esac
    echo

    read -r -p "Enter USB port name of dongle: " USBPORT;
    if [ ! -w "${USBPORT}" ]; then
        echo "ERROR: not a valid/readable port: ${USBPORT}"
        exit 1
    fi
    echo "The USB port is ${USBPORT}"
else
    if [ $# -lt 2 ]; then
        echo "ERROR: USB port is missing..."
        HELP_MSG
    fi

    if [ "$1" == "sdk" ]; then
        FLASHLIST=${APP_DIR}script/flashlist_release_sdk.yml
        LAYOUT=${APP_DIR}script/layout_release_sdk.yml
    elif [ "$1" == "adk" ]; then
        FLASHLIST=${APP_DIR}script/flashlist_release_adk_headset.yml
        LAYOUT=${APP_DIR}script/layout_release_adk_headset.yml
    else
        echo "ERROR: $1 is not a valid board..."
        HELP_MSG
    fi

    USBPORT=$2
    echo "The USB port is ${USBPORT}"
fi

if [ "$3" == "debug" ]; then
    echo ""
    echo "CUR_DIR '${CUR_DIR}'"
    echo "APP_DIR '${APP_DIR}'"
    echo "FLASHTOOL '${FLASHTOOL}'"
    echo "FLASHLIST '${FLASHLIST}'"
    echo "LAYOUT '${LAYOUT}'"
    echo ""
fi

${FLASHTOOL} --work-dir ../ --dev "${USBPORT}" --connection ota --flash-list "${FLASHLIST}" --layout "${LAYOUT}" --partition 0 --activate 0 --flash-only nxh_app,rfmac,cf,kl_app -v
