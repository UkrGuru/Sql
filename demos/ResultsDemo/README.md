## UkrGuru.Sql: Additional Features Under the Hood

In this article, we will explore the powerful features of the `UkrGuru.Sql` library, focusing on its versatile `Results` class. This class provides robust parsing capabilities for various data types, making it an essential tool for developers. Let's dive into some practical examples to demonstrate its functionality.

#### Demonstrating Results class methods:

```csharp
using System;
using System.Text.Json;
using UkrGuru.Sql;

Console.WriteLine("Demonstrating Results class methods:");

// Parsing null values
Console.WriteLine("Parse Null Values:");
Console.WriteLine($"Nullable bool: {Results.Parse<bool?>(null) == null}");
Console.WriteLine($"Non-nullable bool (default false): {Results.Parse<bool>(null)}");
```

**Output:**
```
Parse Null Values:
Nullable bool: True
Non-nullable bool (default false): False
```
This demonstrates how the `Results` class handles null values, providing default values for non-nullable types and correctly identifying nullable types as null.

```csharp
// Parsing integers
object bigInt = 1234567890123456789L;
Console.WriteLine("\nParse BigInt:");
Console.WriteLine($"Parsed long: {Results.Parse<long>(bigInt)}");
Console.WriteLine($"Parsed long from string: {Results.Parse<long>(bigInt.ToString())}");
```

**Output:**
```
Parse BigInt:
Parsed long: 1234567890123456789
Parsed long from string: 1234567890123456789
```
This shows the `Results` class's ability to parse large integers both directly and from their string representations, ensuring accurate conversion.

```csharp
// Parsing strings
object str = "Test";
Console.WriteLine("\nParse String:");
Console.WriteLine($"Parsed string: {Results.Parse<string>(str)}");
```

**Output:**
```
Parse String:
Parsed string: Test
```
Here, the `Results` class successfully parses a simple string, demonstrating its straightforward handling of string data types.

```csharp
// Parsing DateTime
object dateTime = new DateTime(2000, 1, 1, 1, 1, 1);
Console.WriteLine("\nParse DateTime:");
Console.WriteLine($"Parsed DateTime: {Results.Parse<DateTime>(dateTime)}");
Console.WriteLine($"Parsed DateTime from string: {Results.Parse<DateTime>(dateTime.ToString())}");
```

**Output:**
```
Parse DateTime:
Parsed DateTime: 01/01/2000 01:01:01
Parsed DateTime from string: 01/01/2000 01:01:01
```
This example highlights the `Results` class's capability to parse `DateTime` objects both directly and from formatted strings, ensuring precise date and time handling.

```csharp
// Parsing DateOnly
object dateOnly = DateOnly.FromDateTime(DateTime.Now);
Console.WriteLine("\nParse DateOnly:");
Console.WriteLine($"Parsed DateOnly: {Results.Parse<DateOnly>(dateOnly)}");
Console.WriteLine($"Parsed DateOnly from string: {Results.Parse<DateOnly>(dateOnly.ToString())}");
```

**Output:**
```
Parse DateOnly:
Parsed DateOnly: 17/02/2025
Parsed DateOnly from string: 17/02/2025
```
This demonstrates the `Results` class's ability to handle `DateOnly` types, parsing both directly and from string representations, which is useful for date-specific applications.

```csharp
// Parsing TimeOnly
object timeOnly = TimeOnly.FromDateTime(DateTime.Now);
Console.WriteLine("\nParse TimeOnly:");
Console.WriteLine($"Parsed TimeOnly: {Results.Parse<TimeOnly>(timeOnly)}");
Console.WriteLine($"Parsed TimeOnly from string: {Results.Parse<TimeOnly>(timeOnly.ToString())}");
```

**Output:**
```
Parse TimeOnly:
Parsed TimeOnly: 14:28
Parsed TimeOnly from string: 14:28
```
This example shows how the `Results` class can parse `TimeOnly` types, both directly and from formatted strings, making it ideal for time-specific data.

```csharp
// Parsing char[]
object charArray = new char[] { 'A', 'B' };
Console.WriteLine("\nParse char[]:");
Console.WriteLine($"Parsed char[]: {new string(Results.Parse<char[]>(charArray))}");
Console.WriteLine($"Parsed char[] from string: {new string(Results.Parse<char[]>(new string(charArray)))}");
```

**Output:**
```
Parse char[]:
Parsed char[]: AB
Parsed char[] from string: AB
```
This demonstrates the `Results` class's ability to parse character arrays, both directly and from their string representations, ensuring accurate character data handling.

```csharp
// Parsing byte[]
object byteArray = new byte[] { 1, 2, 3 };
Console.WriteLine("\nParse byte[]:");
Console.WriteLine($"Parsed byte[]: {BitConverter.ToString(Results.Parse<byte[]>(byteArray)!)}");
Console.WriteLine($"Parsed byte[] from Base64 string: {BitConverter.ToString(Results.Parse<byte[]>(Convert.ToBase64String(byteArray))!)}");
```

**Output:**
```
Parse byte[]:
Parsed byte[]: 01-02-03
Parsed byte[] from Base64 string: 01-02-03
```
This example highlights the `Results` class's capability to parse byte arrays, both directly and from Base64-encoded strings, which is essential for binary data handling.

```csharp
// Parsing Guid
object guid = Guid.NewGuid();
Console.WriteLine("\nParse Guid:");
Console.WriteLine($"Parsed Guid: {Results.Parse<Guid>(guid)}");
Console.WriteLine($"Parsed Guid from string: {Results.Parse<Guid>(guid.ToString())}");
```

**Output:**
```
Parse Guid:
Parsed Guid: 7ffaa01a-18c8-48e7-9d9b-7de755ebb370
Parsed Guid from string: 7ffaa01a-18c8-48e7-9d9b-7de755ebb370
```
This demonstrates the `Results` class's ability to handle `Guid` types, parsing both directly and from string representations, ensuring unique identifier management.

```csharp
// Parsing JSON
object json = JsonSerializer.Serialize(new { Id = 1, Name = "Test" });
var parsedObject = Results.Parse<JsonElement>(json);
Console.WriteLine("\nParse JSON:");
Console.WriteLine($"Parsed JSON Id: {parsedObject.GetProperty("Id").GetInt32()}");
Console.WriteLine($"Parsed JSON Name: {parsedObject.GetProperty("Name").GetString()}");
```

**Output:**
```
Parse JSON:
Parsed JSON Id: 1
Parsed JSON Name: Test
```
This example shows the `Results` class's capability to parse JSON data, extracting properties from a `JsonElement`, which is crucial for handling structured data.

### Conclusion

The `Results` class from the `UkrGuru.Sql` library offers a wide range of parsing capabilities, making it a valuable tool for developers working with diverse data types. Whether you need to parse null values, integers, strings, dates, times, character arrays, byte arrays, GUIDs, or JSON, the `Results` class has you covered. This demonstration highlights the versatility and power of the `Results` class, showcasing its ability to handle various parsing scenarios with ease.