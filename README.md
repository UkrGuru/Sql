# UkrGuru.Sql
[![Nuget](https://img.shields.io/nuget/v/UkrGuru.Sql)](https://www.nuget.org/packages/UkrGuru.Sql/)
[![Donate](https://img.shields.io/badge/Donate-PayPal-yellow.svg)](https://www.paypal.com/donate/?hosted_button_id=BPUF3H86X96YN)

UkrGuru.Sql is a powerful library designed to simplify interactions between .NET applications and SQL Server databases. It automatically normalizes input parameters and deserializes results, supporting dynamic queries, stored procedures, and asynchronous operations. With UkrGuru.Sql, you can access SQL Server data with minimal code and maximum performance.

## Features

- **Easy Integration**: Seamlessly integrate with ASP.NET Core projects.
- **Dynamic Queries**: Support for dynamic SQL queries and stored procedures.
- **Asynchronous Operations**: Built-in support for async operations.
- **Parameter Normalization**: Automatic normalization of input parameters.
- **Result Deserialization**: Effortless deserialization of query results.

## Installation

To use the UkrGuru.Sql library in your ASP.NET Core project, follow these steps:

1. Install the UkrGuru.Sql package from NuGet:
   ```sh
   dotnet add package UkrGuru.Sql
   ```

2. Update your `appsettings.json` file to include the connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YourDbName;Trusted_Connection=True;"
     }
   }
   ```

3. Register the UkrGuru.Sql services in your `Program.cs` file:

   ```csharp
   using UkrGuru.Sql;
   
   // Set connection string for static use
   DbHelper.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

   var builder = WebApplication.CreateBuilder(args);

   // Register DbService for dependency injection as a scoped service
   builder.Services.AddScoped<IDbService, DbService>();
      
   var app = builder.Build();
   ```

## Samples

```csharp
using UkrGuru.Sql;

// Set the connection string
DbHelper.ConnectionString = "Server=(localdb)\\mssqllocaldb";

// Execute a simple query
var result = DbHelper.Exec<int>("SELECT @A + @B", new { A = 2, B = 2 });
Console.WriteLine($"Result: {result}");

// Work with JSON data
var person = new { Id = 1, Name = "John" };
var name = DbHelper.Exec<string>("SELECT JSON_VALUE(@Data, '$.Name')", person.ToJson());
Console.WriteLine($"Result: {name}");

// Read multiple records
var persons = DbHelper.Read<Person>("SELECT 1 Id, 'John' Name UNION ALL SELECT 2, 'Mike'").ToList();
Console.WriteLine($"Result: {persons.Count}");
Console.WriteLine($"1st person: {persons[0].ToJson()}");
Console.WriteLine($"2nd person: {persons[1].ToJson()}");

// Define the Person class
class Person
{
    public int? Id { get; set; }
    public string? Name { get; set; }
}
```

You can find more examples in the `demos` folder of this repository.

## Contributing

Contributions are welcome! Please fork this repository and submit pull requests.

## License

This project is licensed under the MIT License. See the LICENSE file for details.
