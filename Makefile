all: api dashboard

api:
	sudo docker build -f TempLoggerService.Dashboard.Dockerfile . -t temploggerdashboard

dashboard:
	sudo docker build -f TempLoggerService.Api.Dockerfile . -t temploggerapi

up:
	sudo docker-compose up -d
