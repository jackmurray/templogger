all: api dashboard

api:
	sudo docker build -f TempLoggerService.Dashboard.Dockerfile . -t templogger:dashboard

dashboard:
	sudo docker build -f TempLoggerService.Api.Dockerfile . -t templogger:api

up:
	sudo docker-compose up -d
