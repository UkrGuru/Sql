## Creating a Universal CRUD API with UkrGuru.Sql

In this article, we'll walk through the process of creating a universal CRUD API using ASP.NET Core and UkrGuru.Sql. This API will support various HTTP methods to execute stored procedures, making it a versatile tool for database interactions.

### Setting Up the Project

First, let's set up our ASP.NET Core project. We'll start by creating a new Web Application project:

```bash
dotnet new webapi -n ApiProject
cd ApiProject
```

### Adding Dependencies

Next, we'll add the necessary dependencies to our project. We'll use `UkrGuru.Sql` for database interactions and `Microsoft.AspNetCore.Mvc` for building our API.

```csharp
using Microsoft.AspNetCore.Mvc;
using UkrGuru.Sql;
```

### Defining Constants

We'll define constants for our API pattern and suffix:

```csharp
const string ApiHolePattern = "ApiHole";
const string ApiProcSufix = "_Api";
```

### Configuring Services

We'll configure the services in the `Program.cs` file. This includes registering the database service with dependency injection and adding controllers with a custom input formatter for plain text.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbService, DbService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
```

### Building the Application

Next, we'll build the application and configure the HTTP request pipeline for the development environment. We'll also enforce HTTPS redirection.

```csharp
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
```

### Mapping Endpoints

We'll map the endpoints for executing stored procedures using various HTTP methods (POST, GET, PUT, DELETE).

```csharp
app.MapPost($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, [FromBody] object? data) =>
    await db.TryExecAsync<string?>($"{proc}{ApiProcSufix}", data?.ToJson()));

app.MapGet($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, string? data) =>
    await db.TryExecAsync<string?>($"{proc}{ApiProcSufix}", data));

app.MapPut($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, [FromBody] object? data) =>
    await db.TryExecAsync($"{proc}{ApiProcSufix}", data?.ToJson()));

app.MapDelete($"{ApiHolePattern}/{{proc}}", async (IDbService db, string proc, string? data) =>
    await db.TryExecAsync($"{proc}{ApiProcSufix}", data));
```

### Running the Application

Finally, we'll run the application:

```csharp
app.Run();
```

### Setting Up the Database

To set up the database, use the following T-SQL script to create the stored procedures:

```sql
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Shippers_Del_Api]
    @Data nvarchar(50)
AS
DELETE Shippers 
WHERE CompanyName = @Data
GO

CREATE PROCEDURE [dbo].[Shippers_Get_Api]
    @Data nvarchar(50)
AS
SELECT *
FROM Shippers 
WHERE CompanyName = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
GO

CREATE PROCEDURE [dbo].[Shippers_Ins_Api]
    @Data nvarchar(max)
AS
INSERT INTO Shippers (CompanyName, Phone)
OUTPUT inserted.ShipperID
SELECT CompanyName, Phone 
FROM OPENJSON(@Data) 
    WITH (CompanyName nvarchar(40), Phone nvarchar(24))
GO

CREATE PROCEDURE [dbo].[Shippers_Upd_Api]
    @Data nvarchar(max)
AS
UPDATE Shippers
SET Phone = D.Phone
FROM OPENJSON(@Data) 
    WITH (CompanyName nvarchar(40), Phone nvarchar(24)) D
WHERE Shippers.CompanyName = D.CompanyName
GO
```

### MinSqlApi.http File

To test the API, you can use the following `MinSqlApi.http` file. This file contains HTTP requests for executing stored procedures using different HTTP methods.

```http
@MinSqlApi_HostAddress = http://localhost:5133

### POST request to execute a stored procedure
POST {{MinSqlApi_HostAddress}}/ApiHole/Shippers_Ins
Content-Type: application/json

{ 
  "CompanyName":"Nova Poshta",  
  "Phone":"(800) 111-1111" 
}

### GET request to execute a stored procedure
GET {{MinSqlApi_HostAddress}}/ApiHole/Shippers_Get?data=Nova%20Poshta

### PUT request to execute a stored procedure
PUT {{MinSqlApi_HostAddress}}/ApiHole/Shippers_Upd
Content-Type: application/json

{ 
  "CompanyName":"Nova Poshta",  
  "Phone":"(800) 222-2222" 
}

### DELETE request to execute a stored procedure
DELETE {{MinSqlApi_HostAddress}}/ApiHole/Shippers_Del?data=Nova%20Poshta
```

### Conclusion

By following these steps, you've created a versatile API that can handle various HTTP methods to execute stored procedures. This setup provides a solid foundation for building more complex and feature-rich APIs in the future.
