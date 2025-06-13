#!/bin/bash

# SQL Server'ı başlat
/opt/mssql/bin/sqlservr &

# SQL Server'ın hazır olmasını bekle
sleep 30

# restore.sql içeriğini çalıştır
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "6111098.Cc" -i /restore/restore.sql -N -C

# SQL Server durdurulmadığı sürece container açık kalmalı
wait
