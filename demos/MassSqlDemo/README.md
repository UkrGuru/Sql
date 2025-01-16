### Efficient Bulk Operations with UkrGuru.Sql

In this article, we explore how to perform efficient bulk operations—insert, update, and delete—using the `UkrGuru.Sql` library. This approach is particularly useful when dealing with large datasets, ensuring optimal performance and minimal execution time.

#### Initial Setup

First, we initialize the database connection:

```csharp
Utils.InitDb();
```

We then define the number of records (`N`) and create a list to hold our student data:

```csharp
int N = 100 * 1024; 
List<Student>? students = new();
DateTime started = DateTime.Now;
```

#### Mass Insert

To insert a large number of records efficiently, we prepare the data and use a bulk insert operation:

```csharp
for (int i = 0; i < N; i++)
{
    students.Add(new Student() { ID = i, Name = $"Name_{i}" });
}

var sql_insert = """
    INSERT Students (ID, Name) 
    SELECT D.ID, D.Name FROM OPENJSON(@Data) WITH (ID int, Name varchar(50)) D
    """;

started = DateTime.Now;

await DbHelper.ExecAsync(sql_insert, students.ToJson());

Console.WriteLine($"Inserted {N / 1024}K - {DateTime.Now.Subtract(started)}");
```

This method leverages `OPENJSON` to parse the JSON data and insert it into the `Students` table. The result of this operation was:

```
Database created successfully.
Inserted 100K - 00:00:00.4206026
```

#### Mass Update

Next, we update the records with new class and grade information:

```csharp
for (int i = 0; i < N; i++)
{
    students[i].Class = (char)((byte)'A' + i % 25);
    students[i].Grade = (byte)(i % 5);
}

var sql_update = """
    UPDATE S
    SET S.Class = D.Class, S.Grade = D.Grade
    FROM Students S
    JOIN OPENJSON(@Data) WITH (ID int, Class char(1), Grade tinyint) D ON S.ID = D.ID;
    """;

started = DateTime.Now;

await DbHelper.ExecAsync(sql_update,
    students.Select(c => new { c.ID, c.Class, c.Grade }).ToJson());

Console.WriteLine($"Updated {N / 1024}K - {DateTime.Now.Subtract(started)}");
```

This update operation uses a similar approach, parsing the JSON data and updating the corresponding records in the `Students` table. The result of this operation was:

```
Updated 100K - 00:00:00.3418457
```

#### Mass Delete

Finally, we delete records based on a specific condition (e.g., `Grade < 1`):

```csharp
var sql_delete = """
    DELETE S 
    FROM Students S 
    JOIN OPENJSON(@Data) AS D ON S.ID = D.value;
    """;

started = DateTime.Now;

await DbHelper.ExecAsync(sql_delete,
    students.Where(x => x.Grade < 1).Select(c => c.ID ).ToJson());

Console.WriteLine($"Deleted {(N / 5) / 1024}K - {DateTime.Now.Subtract(started)}");
```

This delete operation ensures that only the records meeting the specified condition are removed from the `Students` table. The result of this operation was:

```
Deleted 20K - 00:00:00.0400413
```

#### Conclusion

Using `UkrGuru.Sql` for bulk operations significantly improves performance when handling large datasets. By leveraging JSON parsing and efficient SQL commands, we can perform mass inserts, updates, and deletes with minimal execution time. The results demonstrate the efficiency of this approach:

- **Database created successfully.**
- **Inserted 100K - 00:00:00.4299062**
- **Updated 100K - 00:00:00.3343855**
- **Deleted 20K - 00:00:00.0399534**

This method ensures that large-scale data operations are handled swiftly and effectively, making it an excellent choice for high-performance applications.