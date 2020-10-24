INSERT INTO TemperatureLog.dbo.Temperatures (DeviceId, Timestamp, Value)
SELECT deviceId AS DeviceId, timestamp as Timestamp, value as Value
FROM TemperatureLog_export.dbo.temperature
ORDER BY timestamp ASC