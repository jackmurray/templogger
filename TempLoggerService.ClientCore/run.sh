#!/bin/bash
for dev in `ls /dev/disk/by-partlabel`;
do
        RAW=`/usr/sbin/smartctl -A /dev/disk/by-partlabel/$dev | grep ^194`
        if [ $? -eq 0 ]; then
                TEMPERATURE=`echo $RAW | awk '{print $10}'`
                /root/TempLoggerService.ClientCore/TempLoggerService.ClientCore --set $dev $TEMPERATURE
        fi
done
CPUTEMP=`sensors | grep Package | awk '{print $4}' | grep -E -o "[0-9.]+"`
/root/TempLoggerService.ClientCore/TempLoggerService.ClientCore --set nas4free_cpu $CPUTEMP
