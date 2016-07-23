#!/bin/bash
for dev in `ls /dev/disk/by-partlabel`;
do
        TEMPERATURE=`/usr/sbin/smartctl -A /dev/disk/by-partlabel/$dev | grep ^194 | awk '{print $10}'`
        /usr/bin/mono /root/temploggerclient/TempLoggerService.Client.exe --set $dev $TEMPERATURE
done
CPUTEMP=`sensors | grep Physical | awk '{print $4}' | grep -E -o "[0-9.]+"`
/usr/bin/mono /root/temploggerclient/TempLoggerService.Client.exe --set nas4free_cpu $CPUTEMP
