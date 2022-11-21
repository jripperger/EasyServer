# EasyServer

## Overview
This application is intended to simplify the job of restaurant servers by implementing a queue to organize customer service.

EasyServer requires the installation of .NET MAUI to one's system. Currently, the application runs exclusively on Windows machines, but other 
platforms will likely be compatible in the future (i.e. Mac, iOS, and Android). Since this app has just been implemented locally, 
it is only executable via an IDE; during this application's development process, Visual Studio 2022 Community was used. Furthermore, 
EasyServer does not utilize a remotely accessible backend server. The database used for testing this app was simply a Microsoft
SQL Server Express LocalDB. Thus, users must configure their own Database and connect it to the application themselves. The necessary 
database configuration is described below.

## Database Configuration
Some variant of SQL Server is needed.

### Employees table
Columns: EmployeeId (PK), FirstName, LastName, Username, Password, Role ("Server" or "Manager")

### RestaurantTables table
Columns: TableId (PK), QrCode, EmployeeId (FK to Employees table EmployeeId)

### Object-Relational Mapper
In the development of this application, Microsoft's Entity Framework was used to convert the aforementioned database columns into C# objects.
Using the Visual Studio's EF Core Power Tools extension, a database context file and the required classes can be generated in just a few steps.
Once the extension has been installed and the EasyServer repository has been opened in Visual Studio:

1. Right click on the project
2. Select "Reverse Engineer"
3. Add a server connection (the name of your created database) and select the desired database 
4. Click "Next"
5. Add a route for the generated files to be placed
6. Click "Done"
7. The files will now appear in the previously specified route
8. If the database was configured properly, one can simply delete the "EasyServerContext" file in the "EasyServerDB" folder of the project
   and replace it with their newly created database context file; the "Employees" and "RestaurantTables" classes should not need to be deleted
