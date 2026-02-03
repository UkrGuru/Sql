using UkrGuru.Sql;
using static System.Runtime.InteropServices.JavaScript.JSType;

// Set the connection string for the database
DbHelper.ConnectionString = "Server=(localdb)\\mssqllocaldb";

var tsql = """
    SELECT CAST(SUM(TRY_CONVERT(decimal(18,4), quantity) * TRY_CONVERT(decimal(18,4), price)) AS decimal(18,2))
    FROM OPENJSON(@Data, '$.items')
    WITH (
        quantity int    '$.quantity',
        price    decimal(18,4) '$.price'
    )
    """;
var data = """
    {
      "orderId": "12345",
      "restaurant": "McDonald''s",
      "items": [
        { "name": "Big Mac",       "quantity": 1, "price": 5.99 },
        { "name": "Medium Fries",  "quantity": 1, "price": 2.49 },
        { "name": "Coca-Cola",     "size": "Medium", "quantity": 1, "price": 1.99 }
      ]
    }
    
    """;
var result = DbHelper.Exec<decimal>(tsql, data);
Console.WriteLine($"Result: {result}");


//// Execute a simple SQL query with parameters and return an integer result
//var result = DbHelper.Exec<int>("SELECT @A + @B", new { A = 2, B = 2 });
//Console.WriteLine($"Result: {result}");

//// Work with JSON-like anonymous object and retrieve a string value
//var person = new { Id = 1, Name = "John" };
//var name = DbHelper.Exec<string>("SELECT @Name", person);
//Console.WriteLine($"\r\nResult: {name}");

//// Read multiple records from a SQL query and map them to a list of Person objects
//var persons = DbHelper.Read<Person>("""
//    SELECT 1 Id, 'John' Name
//    UNION ALL SELECT 2, 'Mike'
//    """).ToList();
//Console.WriteLine($"\r\nResult: {persons.Count}");
//Console.WriteLine($"1st person: {persons[0].ToJson()}");
//Console.WriteLine($"2nd person: {persons[1].ToJson()}");

//// Define the Person class to match the structure of the SQL result
//class Person
//{
//    public int? Id { get; set; }
//    public string? Name { get; set; }
//}


