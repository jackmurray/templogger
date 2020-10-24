INSERT INTO TemperatureLog.dbo.Devices (DeviceId, DeviceName)
SELECT deviceId AS DeviceId, deviceName as DeviceName
FROM TemperatureLog_export.dbo.device