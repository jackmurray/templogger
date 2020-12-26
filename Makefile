all: build

build:
	sudo pwsh Run-CI.ps1 -Mode Build
up:
	sudo docker-compose up -d
push:
	sudo pwsh Run-CI.ps1 -Mode Push
