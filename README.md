# UkrGuru.Sql
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.Sql)](https://www.nuget.org/packages/UkrGuru.Sql/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

UkrGuru.Sql is a library that makes it easy to interact between .NET applications and SQL Server databases. UkrGuru.Sql automatically normalizes input parameters and deserializes the result. Supports dynamic queries, stored procedures and asynchronous operations. With the UkrGuru.Sql package, you can access SQL Server data with minimal code and maximum performance.

## Installation

To use UkrGuru Sql library in your ASP.NET Core project, you need to follow these steps:

### 1. Install the UkrGuru.Sql package from NuGet.

### 2. Open the AppSettings.json files and add the "UkrGuru.Sql" package and "DefaultConnection" elements.

```json
{
    ***
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDbName;Trusted_Connection=True;"
  }
    ***
}
```

### 2. Open the Program.cs file and register the UkrGuru.Sql services and extensions:

```c#
using UkrGuru.Sql;

DbHelper.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbService, DbService>();;

// More other services here ... 

var app = builder.Build();
```

## Samples of code

```c#


```
