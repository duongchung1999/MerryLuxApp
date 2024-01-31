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

CUR_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
FLASHTOOL=${CUR_DIR}/flashtool.cmd
WORK_DIR=${CUR_DIR}/..
PERSIST_DATAGENTOOL=${CUR_DIR}/persist_datagen.cmd
PERSIST_BIN_FILE=${CUR_DIR}/persist_data.bin
FACT_PERSIST_BIN_FILE=${CUR_DIR}/nxh_pers_factory_data.bin
DEV_CONF_YAML=${CUR_DIR}/nxh_device_info.yml


read -r -n 1 -p "Flashing Dongle[D], OTA_Dongle[O], Analogue_Dongle[A], Dongle_Mix[M] or Headset[H] or Headset_Usb[U] application?" option;
case ${option} in
    o|O) _app=ota_dongle;;
    d|D) _app=dongle;;
    a|A) _app=dongle_analogue;;
    m|M) _app=dongle_mix;;
    h|H) _app=headset;;
    u|U) _app=headset_usb;;
    *) exit 1;;
esac
echo

if [ "${_app}" != "dongle_analogue" ]; then
    read -r -n 1 -p "Flashing an ADK[A] or SDK[S] board?" option;
    case ${option} in
        a|A) _kl_bsp=adk;;
        s|S) _kl_bsp=sdk;;
        *) exit 1;;
    esac
    echo
else
    echo ""
    echo "Warning: Analogue_Dongle application can only be flashed to an SDK board."
    echo "         Please be sure that an SDK board is being used, not an ADK board."
    echo ""
    _kl_bsp=sdk
fi

read -r -n 1 -p "Flashing Release[R] or Debug[D]?" option;
case ${option} in
    r|R) _kl_target=release;;
    d|D) _kl_target=debug;;
    *) exit 1;;
esac
echo

if [ "${_app}" != "ota_dongle" ]; then
    echo "Flash memory will be completely erased before flashing."
    read -r -n 1 -p  "Do you like to Keep[K] or Write[W] new nxh persistent data (BD address, ir, er, ...) " option;
    case ${option} in
        k|K) _persist_data_action=keep_persist_data;;
        w|W) _persist_data_action=write_persist_data;;
        *) exit 1;;
    esac
    echo
    if [ "${_persist_data_action}" == "write_persist_data" ]; then
        read -r -n 1 -p  "Do you like to Generate Random device info[G] or Write it from 'nxh_device_info.yml'[W]" option;
        case ${option} in
            g|G) _persist_data_action=gen_persist_data;;
            w|W) _persist_data_action=write_persist_data;;
            *) exit 1;;
        esac
        echo
    fi
else
    echo ""
    echo "Current persistent data will be kept automatically for OTA_Dongle"
    _persist_data_action=keep_persist_data
fi

_kl_app=kl_${_app}
APP_DIR=${CUR_DIR}/../kinetis_democode/apps/${_kl_app}
_flashlist_bsp=${_kl_bsp}
if [ "${_kl_bsp}" == "adk" ]; then
    _flashlist_bsp=adk_${_app}
    if [ "${_app}" == "ota_dongle" ]; then
        _flashlist_bsp=adk_dongle
    fi
    if [ "${_app}" == "dongle_mix" ]; then
        _flashlist_bsp=adk_dongle
    fi
    if [ "${_app}" == "headset_usb" ]; then
        _flashlist_bsp=adk_headset
    fi
fi

FLASHLIST=${APP_DIR}/script/flashlist_${_kl_target}_${_flashlist_bsp}.yml
LAYOUT=${APP_DIR}/script/layout_${_kl_target}_${_flashlist_bsp}.yml

if [ "${_persist_data_action}" == "gen_persist_data" ]; then
    echo "Device info is being generated..."
    "${PERSIST_DATAGENTOOL}" --generate-devinfo "${FACT_PERSIST_BIN_FILE}" || ret=$false
    if [ ! -e "${FACT_PERSIST_BIN_FILE}" ]; then
        echo ""
        echo "ERROR! Factory Persistent data could not be written into ${FACT_PERSIST_BIN_FILE}"
        exit 1
    fi
fi

if [ "${_persist_data_action}" == "write_persist_data" ]; then
    echo "Device info is being written from nxh_device_info.yml file"
    "${PERSIST_DATAGENTOOL}" --write-devinfo "${FACT_PERSIST_BIN_FILE}" --devinfo "$DEV_CONF_YAML" || ret=$false
    if [ ! -e "${FACT_PERSIST_BIN_FILE}" ]; then
        echo ""
        echo "ERROR! Factory Persistent data could not be written into ${FACT_PERSIST_BIN_FILE}"
        exit 1
    fi
fi

if [ "${_persist_data_action}" == "keep_persist_data" ]; then
    echo "Current persistent data in the device is being backed up"
    "${FLASHTOOL}" --work-dir "$WORK_DIR" --dev kl --connection usb --dump-to-file "${PERSIST_BIN_FILE}" --partition nxh_pers_data || ret=$false
    if [ ! -e "${PERSIST_BIN_FILE}" ]; then
        echo ""
        echo "ERROR! Persistent data could not be dumped into ${PERSIST_BIN_FILE}"
        echo "Partition table might be outdated, or flash is empty"
        exit 1
    fi

    "${FLASHTOOL}" --work-dir "$WORK_DIR" --dev kl --connection usb --dump-to-file "${FACT_PERSIST_BIN_FILE}" --partition nxh_pers_factry || ret=$false
    if [ ! -e "${FACT_PERSIST_BIN_FILE}" ]; then
        echo ""
        echo "ERROR! Factory Persistent data could not be dumped into ${FACT_PERSIST_BIN_FILE}"
        echo "Partition table might be outdated, or flash is empty"
        exit 1
    fi
fi


"${FLASHTOOL}" --work-dir "$WORK_DIR" --dev kl --connection usb --erase-chip --flash-list "${FLASHLIST}" --layout "${LAYOUT}" -v

if [ "${_persist_data_action}" == "keep_persist_data" ]; then
    "${FLASHTOOL}" --work-dir "$WORK_DIR" --dev kl --connection usb --restore-from-file "${PERSIST_BIN_FILE}" --partition nxh_pers_data
    rm "${PERSIST_BIN_FILE}"
fi

"${FLASHTOOL}" --work-dir "$WORK_DIR" --dev kl --connection usb --restore-from-file "${FACT_PERSIST_BIN_FILE}" --partition nxh_pers_factry
rm -f "${FACT_PERSIST_BIN_FILE}"

"${FLASHTOOL}" --work-dir "$WORK_DIR" --dev kl --connection usb --reset