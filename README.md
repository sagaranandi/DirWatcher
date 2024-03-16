# Directory Watcher

Directory Watcher is a tool that monitors a specific directory for changes and performs actions accordingly.

## Overview

The Directory Watcher project consists of a background service that continuously monitors a directory, detects changes such as file additions, deletions, and modifications, and logs these changes in a database. Additionally, it provides an API for configuring the directory to monitor, the magic string to detect in files, and starting or stopping the monitoring process.
I have used SQLite DB.

## Prerequisites

Before running the project, ensure you have the following installed:

- .NET Core SDK
- SQL Server or compatible database server
- Swagger enabled

## Installation

1. Clone this repository to your local machine.
2. Navigate to the project directory.
3. Configure the database connection string in the `appsettings.json` file.
4. Run the database migrations to create the required tables.
5. Start the application.

# API Endpoints

| Route                                   | Method | Body                                  | Sample Response                          | Description                               |
|-----------------------------------------|--------|---------------------------------------|------------------------------------------|-------------------------------------------|
| /api/Configuration/ConfigureDirectory   | POST   | {"directory": "/path/to/directory"}   | Success    200 OK                        | Set the directory configuration.          |
| /api/Configuration/ConfigureMagicString | POST   | {"magicString": "exampleMagicString"} | Success    200 OK                        | Set the magic string configuration.       |
| /api/Configuration/StartMonitoring      | POST   | N/A                                   | Success    200 OK                        | Start monitoring the directory.           |
| /api/Configuration/StopMonitoring       | POST   | N/A                                   | Success    200 OK                        | Stop monitoring the directory.            |

