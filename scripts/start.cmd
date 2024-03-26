@echo off
cd ..\Valuator\
start dotnet run --urls "http://0.0.0.0:5001"
start dotnet run --urls "http://0.0.0.0:5002"

cd ..\nginx REM Путь к nginx
start nginx -c .\conf\nginx.conf REM Путь к nginx.conf