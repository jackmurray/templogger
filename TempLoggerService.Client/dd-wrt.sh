#!/bin/sh
DATE=`date "+%Y-%m-%d %H:%M:%S"`

GETTEMP1=`wl -i eth1 phy_tempsense | awk '{ print $1}'`
TEMP24=$((GETTEMP1/2+20))

GETTEMP2=`wl -i eth2 phy_tempsense | awk '{ print $1}'`
TEMP50=$((GETTEMP2/2+20))

curl -X POST http://templogger.corp.c0rporation.com/TempLoggerService/api/temperature/LogTemp -d "{\"temp\": \"$TEMP24\", \"device\": \"825295CD-8585-4BAD-8F1A-22FD58ED8E8F\", \"timestamp\": \"$DATE\"}" --header "Content-Type: application/json"
curl -X POST http://templogger.corp.c0rporation.com/TempLoggerService/api/temperature/LogTemp -d "{\"temp\": \"$TEMP50\", \"device\": \"315D4DDB-49CF-43E2-9860-9581C1EE21FB\", \"timestamp\": \"$DATE\"}" --header "Content-Type: application/json"
