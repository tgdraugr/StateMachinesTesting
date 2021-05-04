#!/bin/bash
docker-compose up --build --detach

WAIT_TIME=10s
echo "Waiting ${WAIT_TIME} to \"warmup\" the containers..."
sleep ${WAIT_TIME}

echo "---------------------------------------------"
echo "- SQL Server Migration"
echo "---------------------------------------------"
dotnet ef database update -c GreetingsDbContext --project StateMachines.SqlServer

echo "---------------------------------------------"
echo "- PostgreSQL Migration"
echo "---------------------------------------------"
dotnet ef database update -c GreetingsDbContext --project StateMachines.PostgreSql

echo "---------------------------------------------"
echo "- MySql (by Oracle) Migration"
echo "---------------------------------------------"
dotnet ef database update -c GreetingsDbContext --project StateMachines.MySql.Oracle

echo "---------------------------------------------"
echo "- MySql (by Pomelo Foundation) Migration"
echo "---------------------------------------------"
dotnet ef database update -c GreetingsDbContext --project StateMachines.MySql.PomeloFoundation