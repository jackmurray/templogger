param(
    [Parameter(Mandatory)][ValidateSet("Build", "Push", "All")][string]$Mode
)

if ($Mode -in @("Build", "All"))
{
    & docker build -f TempLoggerService.Dashboard.Dockerfile . -t templogger:dashboard
    & docker build -f TempLoggerService.Api.Dockerfile . -t templogger:api
}

if ($Mode -in @("Push", "All"))
{
    & docker tag templogger:api jackjcmurray/templogger:api
    & docker tag templogger:dashboard jackjcmurray/templogger:dashboard

    & docker image push jackjcmurray/templogger:api
    & docker image push jackjcmurray/templogger:dashboard
}
