## Guide to Using Parameters in UkrGuru.Sql

In this article, we'll walk through creating a sample console application using the **UkrGuru.Sql** library. This library simplifies database operations in .NET applications. We'll demonstrate how to execute various SQL queries and handle different data types.

### Setting Up the Project

First, ensure you have the **UkrGuru.Sql** library installed. You can add it to your project via NuGet Package Manager.

```bash
dotnet add package UkrGuru.Sql
```

### Configuring the Connection String

Set up your database connection string. For this example, we'll use a local database.

```csharp
DbHelper.ConnectionString = "Server=(localdb)\\mssqllocaldb;Integrated Security=true";
```

### Executing SQL Queries

The **UkrGuru.Sql** library uses `@Data` as the default name for a single parameter. Below are examples of executing SQL queries with different data types.

#### Boolean Result

To execute a query that returns a boolean result:

```csharp
var boolResult = DbHelper.Exec<bool>("SELECT @Data", true);
Console.WriteLine($"Boolean Result: {boolResult}");
```

**Result**: `Boolean Result: True`

#### Integer Result

For queries that return an integer:

```csharp
var intResult = DbHelper.Exec<int>("SELECT @Data", 123);
Console.WriteLine($"Integer Result: {intResult}");
```

**Result**: `Integer Result: 123`

#### String Result

To handle string results:

```csharp
var stringResult = DbHelper.Exec<string>("SELECT @Data", "Hello, World!");
Console.WriteLine($"String Result: {stringResult}");
```

**Result**: `String Result: Hello, World!`

#### Field Result

When you need to work with named parameters:

```csharp
var fieldResult = DbHelper.Exec<string>("SELECT @Name", new { Name = "John" });
Console.WriteLine($"Field Result: {fieldResult}");
```

**Result**: `Field Result: John`

#### DateOnly Result

For date-only results:

```csharp
var dateResult = DbHelper.Exec<DateOnly>("SELECT @Data", DateTime.Today);
Console.WriteLine($"DateOnly Result: {dateResult}");
```

**Result**: `DateOnly Result: 05/01/2025`

#### TimeOnly Result

To handle time-only results:

```csharp
var timeResult = DbHelper.Exec<TimeOnly>("SELECT @Data", new TimeSpan(23, 59, 0));
Console.WriteLine($"TimeOnly Result: {timeResult}");
```

**Result**: `TimeOnly Result: 23:59`

#### Decimal Result

For queries that return a decimal:

```csharp
var decimalResult = DbHelper.Exec<decimal>("SELECT @Data", 123.45m);
Console.WriteLine($"Decimal Result: {decimalResult}");
```

**Result**: `Decimal Result: 123.45`

#### Guid Result

To handle GUID results:

```csharp
var guidResult = DbHelper.Exec<Guid>("SELECT @Data", Guid.NewGuid());
Console.WriteLine($"Guid Result: {guidResult}");
```

**Result**: `Guid Result: b69df053-cf79-4689-a6b2-93f390dd705d`

#### Enum Result

When working with enums:

```csharp
var enumResult = DbHelper.Exec<UserType>("SELECT @Data", UserType.Admin);
Console.WriteLine($"Enum Result: {enumResult}");
```

**Result**: `Enum Result: Admin`

### Handling JSON and Custom Types

You can also handle JSON and custom types with **UkrGuru.Sql**.

```csharp
// JSON Object Result
var jsonObject = DbHelper.Exec<NamedType>("SELECT @Id Id, @Name Name FOR JSON PATH, WITHOUT_ARRAY_WRAPPER", new { Id = 1, Name = "Test" });
Console.WriteLine($"Json Object Result: Id = {jsonObject?.Id}, Name = {jsonObject?.Name}");
```

**Result**: `Json Object Result: Id = 1, Name = Test`

```csharp
// Named Object Result
var namedObject = DbHelper.Read<NamedType>("SELECT @Id Id, @Name Name", new { Id = 1, Name = "Test" }).FirstOrDefault();
Console.WriteLine($"Named Object Result: Id = {namedObject?.Id}, Name = {namedObject?.Name}");
```

**Result**: `Named Object Result: Id = 1, Name = Test`

## Using SqlParameter and SqlParameter[]

In addition to the default parameter handling, you can also use `SqlParameter` and `SqlParameter[]` for more complex scenarios.

### Using SqlParameter

You can create individual `SqlParameter` objects to pass parameters to your SQL queries.

```csharp
var sqlBooleanParam = new SqlParameter("@Data", true);
var booleanResult = DbHelper.Exec<bool>("SELECT @Data", sqlBooleanParam);
Console.WriteLine($"Sql Boolean Result: {booleanResult}");
```

**Result**: `Sql Boolean Result: True`

### Using SqlParameter[]

For queries that require multiple parameters, you can use an array of `SqlParameter` objects.

```csharp
var sqlNamedParameters = new SqlParameter[]
{
    new SqlParameter("@Id", 1),
    new SqlParameter("@Name", "John")
};
var sqlNamedResult = DbHelper.Read<NamedType>("SELECT @Id Id, @Name Name", sqlNamedParameters).FirstOrDefault(); 
Console.WriteLine($"Sql Named Result: Id = {sqlNamedResult?.Id}, Name = {sqlNamedResult?.Name}");
```

**Result**: `Sql Named Result: Id = 1, Name = John`

### Enum and Custom Class Definitions

Define your enums and custom classes as needed.

```csharp
enum UserType
{
    Guest,
    User,
    Manager,
    Admin,
    SysAdmin
}

class NamedType
{
    public int Id { get; set; }
    public string? Name { get; set; }
}
```

### Conclusion

This sample console application demonstrates how to use the **UkrGuru.Sql** library to execute SQL queries and handle various data types in a .NET application. With this library, you can simplify your database operations and focus on building your application's core features.

Feel free to expand this example to suit your specific needs. Happy coding! 🚀
