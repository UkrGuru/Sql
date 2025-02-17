using System;
using System.Text.Json;
using UkrGuru.Sql;

Console.WriteLine("Demonstrating Results class methods:");

// Parsing null values
Console.WriteLine("Parse Null Values:");
Console.WriteLine($"Nullable bool: {Results.Parse<bool?>(null) == null}");
Console.WriteLine($"Non-nullable bool (default false): {Results.Parse<bool>(null)}");

// Parsing integers
object bigInt = 1234567890123456789L;
Console.WriteLine("\nParse BigInt:");
Console.WriteLine($"Parsed long: {Results.Parse<long>(bigInt)}");
Console.WriteLine($"Parsed long from string: {Results.Parse<long>(bigInt.ToString())}");

// Parsing strings
object str = "Test";
Console.WriteLine("\nParse String:");
Console.WriteLine($"Parsed string: {Results.Parse<string>(str)}");

// Parsing DateTime
object dateTime = new DateTime(2000, 1, 1, 1, 1, 1);
Console.WriteLine("\nParse DateTime:");
Console.WriteLine($"Parsed DateTime: {Results.Parse<DateTime>(dateTime)}");
Console.WriteLine($"Parsed DateTime from string: {Results.Parse<DateTime>(dateTime.ToString())}");

// Parsing DateOnly
object dateOnly = DateOnly.FromDateTime(DateTime.Now);
Console.WriteLine("\nParse DateOnly:");
Console.WriteLine($"Parsed DateOnly: {Results.Parse<DateOnly>(dateOnly)}");
Console.WriteLine($"Parsed DateOnly from string: {Results.Parse<DateOnly>(dateOnly.ToString())}");

// Parsing TimeOnly
object timeOnly = TimeOnly.FromDateTime(DateTime.Now);
Console.WriteLine("\nParse TimeOnly:");
Console.WriteLine($"Parsed TimeOnly: {Results.Parse<TimeOnly>(timeOnly)}");
Console.WriteLine($"Parsed TimeOnly from string: {Results.Parse<TimeOnly>(timeOnly.ToString())}");

// Parsing char[]
object charArray = new char[] { 'A', 'B' };
Console.WriteLine("\nParse char[]:");
Console.WriteLine($"Parsed char[]: {new string(Results.Parse<char[]>((char[])charArray))}");
Console.WriteLine($"Parsed char[] from string: {new string(Results.Parse<char[]>(new string((char[])charArray)))}");

// Parsing byte[]
object byteArray = new byte[] { 1, 2, 3 };
Console.WriteLine("\nParse byte[]:");
Console.WriteLine($"Parsed byte[]: {BitConverter.ToString(Results.Parse<byte[]>((byte[])byteArray)!)}");
Console.WriteLine($"Parsed byte[] from Base64 string: {BitConverter.ToString(Results.Parse<byte[]>(Convert.ToBase64String((byte[])byteArray))!)}");

// Parsing Guid
object guid = Guid.NewGuid();
Console.WriteLine("\nParse Guid:");
Console.WriteLine($"Parsed Guid: {Results.Parse<Guid>(guid)}");
Console.WriteLine($"Parsed Guid from string: {Results.Parse<Guid>(guid.ToString())}");

// Parsing JSON
object json = JsonSerializer.Serialize(new { Id = 1, Name = "Test" });
var parsedObject = Results.Parse<JsonElement>(json);
Console.WriteLine("\nParse JSON:");
Console.WriteLine($"Parsed JSON Id: {parsedObject.GetProperty("Id").GetInt32()}");
Console.WriteLine($"Parsed JSON Name: {parsedObject.GetProperty("Name").GetString()}");