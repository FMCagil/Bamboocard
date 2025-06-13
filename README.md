# Bamboocard

# Deployment Guide

This project uses Docker to facilitate seamless setup and deployment. With Docker Compose, both the SQL Server and ASP.NET Core web application are automatically configured and started together. The SQL Server container includes an initialization script that restores the database from a backup during startup.

## Requirements
- Docker and Docker Compose must be installed

## Setup and Launch

### 1. Clone the repository (if applicable)

bash git clone <your-repo-url> cd <your-repo-directory>


### 2. Launch all services
Run the following command to build and start all containers:


bash docker-compose up -d


This command will automatically set up the SQL Server and the web application. The SQL container will execute the `init/sh` script inside the `init` folder, which restores the database from your backup files.

## Inside the `init` Folder
The `init` folder contains the following script:


bash #!/bin/bash


# Start SQL Server
/opt/mssql/bin/sqlservr &
# Wait until SQL Server is ready
sleep 30
# Execute the restore script
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "6111098.Cc" -i /restore/restore.sql -N -C
# Keep the container running as long as SQL Server is active
wait


This script automatically restores the database when the container starts and keeps the container alive.

## Notes
- The volume mappings and paths in your `docker-compose.yml` ensure that the script and backup files are correctly mounted.
- The `init.sh` script should be configured to run automatically during container startup. This can be set up in your Dockerfile or `docker-compose.yml`.

## Additional Commands
- To stop the services:

bash docker-compose down


- To rebuild and restart everything:


bash docker-compose build docker-compose up -d



By following these steps, your SQL Server and web application will be automatically deployed, with the database restored from your backup, ready for use.

---

If you'd like, I can help you refine your `docker-compose.yml` or any other configuration files. Feel free to ask for further assistance!
