#!/bin/sh
GETTEMP1=`wl -i eth1 phy_tempsense | awk '{ print $1}'`
TEMP24=$((GETTEMP1/2+20))

GETTEMP2=`wl -i eth2 phy_tempsense | awk '{ print $1}'`
TEMP50=$((GETTEMP2/2+20))

curl --cacert ca.crt -X POST https://app.corp.c0rporation.com:8080/templogger/api/temperature -d "{\"Value\": \"$TEMP24\", \"DeviceId\": \"825295CD-8585-4BAD-8F1A-22FD58ED8E8F\"}" --header "Content-Type: application/json"
curl --cacert ca.crt -X POST https://app.corp.c0rporation.com:8080/templogger/api/temperature -d "{\"Value\": \"$TEMP50\", \"DeviceId\": \"315D4DDB-49CF-43E2-9860-9581C1EE21FB\"}" --header "Content-Type: application/json"
