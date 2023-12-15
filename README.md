# Password Manager with MongoDB Backend

This repository contains a comparative project that shifts the backend of an existing relational password manager to use MongoDB, focusing on bankcards of specific groups.


![keanu](https://github.com/AlmaLoeb/MongoDBVergleichsprojektLoeblich/assets/80312433/49ce9a7c-117a-47c1-a672-10c3bc319eba)


## Project Overview

This project is part of a comparative analysis between a relational database system and a MongoDB implementation. The core of this project involves implementing CRUD operations and performance benchmarking against both database systems. The MongoDB implementation is designed with a frontend-optimized approach.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

Before you begin, ensure you have the following installed:
- [Docker](https://www.docker.com/get-started)
- [Visual Studio](https://visualstudio.microsoft.com/downloads/) with the `.NET Desktop Development` workload.

### Setting up the Development Environment

1. **Create Docker Containers for the Database Services:**

   - **SQL Server Container:**

     ```sh
     docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=SqlServer2019' -p 1433:1433 --name sql-server-container -d mcr.microsoft.com/mssql/server:2019-latest
     ```

   - **MongoDB Container:**

     ```sh
     docker run -d -p 27017:27017 --name mongo-container mongo:latest
     ```

2. **Clone the Repository:**

   Clone the project repository to your local machine using the following command:

   ```sh
   git clone https://github.com/yourusername/MongoDBVergleichsprojektLoeblich.git
   ```
3. **Open the Solution in Visual Studio:**

Navigate to the directory where you cloned the repository and open the .sln file in Visual Studio.
  ```sh
   cd MongoDBVergleichsprojektLoeblich
   start MongoDBVergleichsprojektLoeblich.sln
  ```

4. **Run the PasswordManager.Mongo Project:**

In Visual Studio, set the PasswordManager.Mongo as the startup project and run it. This will perform the CRUD performance tests and seed the database as required.
