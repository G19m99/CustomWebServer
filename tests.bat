@echo off

start cmd /k "curl -i http://localhost:8080/ && pause"
start cmd /k "curl -i http://localhost:8080/index && pause"
start cmd /k "curl -i http://localhost:8080/index.html && pause"
start cmd /k "curl -i http://localhost:8080/index.txt && pause"
start cmd /k "curl -i http://localhost:8080/test && pause"
start cmd /k "curl -i http://localhost:8080/json && pause"
