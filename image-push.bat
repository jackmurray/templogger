docker tag templogger:api jackjcmurray/templogger:api
docker tag templogger:dashboard jackjcmurray/templogger:dashboard

docker image push jackjcmurray/templogger:api
docker image push jackjcmurray/templogger:dashboard
