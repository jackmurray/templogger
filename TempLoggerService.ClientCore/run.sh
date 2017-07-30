#!/bin/bash
for dev in `ls /dev/disk/by-partlabel`;
do
        TEMPERATURE=`/usr/sbin/smartctl -A /dev/disk/by-partlabel/$dev | grep ^194 | awk '{print $10}'`
        /root/TempLoggerService.ClientCore/dotnet/dotnet /root/TempLoggerService.ClientCore/client/TempLoggerService.ClientCore.dll  --set $dev $TEMPERATURE
done
CPUTEMP=`sensors | grep Package | awk '{print $4}' | grep -E -o "[0-9.]+"`
/root/TempLoggerService.ClientCore/dotnet/dotnet /root/TempLoggerService.ClientCore/client/TempLoggerService.ClientCore.dll --set nas4free_cpu $CPUTEMP
