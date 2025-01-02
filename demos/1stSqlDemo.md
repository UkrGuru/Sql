## Introduction to `UkrGuru.Sql` with a Demo Application

In this article, we will explore the `UkrGuru.Sql` library by creating a simple console application that performs various database operations. This demo app will guide you through creating a database, managing a table, and performing CRUD (Create, Read, Update, Delete) operations. `UkrGuru.Sql` is a powerful library that simplifies database interactions, making it easier for developers to focus on the core logic of their applications.

### Setting Up the Connection

First, we need to set up the connection string for our database. We'll use a local SQL Server instance for this demo. The connection string includes the server name and security settings. We will also specify the database name later.

```csharp
var BaseConnectionString = "Server=(localdb)\\mssqllocaldb;Integrated Security=true";
var DatabaseName = "UkrGuruSqlDemo";

// Setting the connection string for DbHelper
DbHelper.ConnectionString = $"{BaseConnectionString};Database={DatabaseName};";
```

### Main Application Loop

The main loop of our console application will present a menu to the user, allowing them to select different database operations. This loop will continue running until the user chooses to exit.

```csharp
while (true)
{
    // Clear the console to keep the questions fixed at the top
    Console.Clear();

    // Display the menu options
    Console.WriteLine("Select an operation:");
    Console.WriteLine("1. Create database");
    Console.WriteLine("2. Create Persons table");
    Console.WriteLine("3. Insert record");
    Console.WriteLine("4. Read 1st record");
    Console.WriteLine("5. Read all records");
    Console.WriteLine("6. Count of records");
    Console.WriteLine("7. Update 1st record");
    Console.WriteLine("8. Delete 1st record");
    Console.WriteLine("9. Delete database");
    Console.WriteLine("0. Exit");

    // Prompt the user to choose an option
    Console.WriteLine("\nPlease choose an option from the list above to proceed with the corresponding database operation.");
    Console.Write("\nEnter your choice: ");
    Int32.TryParse(Console.ReadKey().KeyChar.ToString(), out int choice);
    Console.WriteLine("");

    // Execute the selected operation
    switch (choice)
    {
        case 1:
            CreateDatabase(BaseConnectionString, DatabaseName);
            break;
        case 2:
            CreateTable();
            break;
        case 3:
            InsertRandomRecord();
            break;
        case 4:
            Read1stRecord();
            break;
        case 5:
            ReadAllRecords();
            break;
        case 6:
            CountRecords();
            break;
        case 7:
            Update1stRecord();
            break;
        case 8:
            Delete1stRecord();
            break;
        case 9:
            DeleteDatabase(BaseConnectionString, DatabaseName);
            break;
        case 0:
            return;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }

    // Pause to allow the user to see the selected option before clearing the screen
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}
```

### Database Operations

#### Create Database

This method creates a new database using the provided connection string. It first connects to the master database, executes the `CREATE DATABASE` command, and then switches back to the newly created database.

```csharp
static void CreateDatabase(string baseConnectionString, string databaseName)
{
    try
    {
        // Connect to the master database to create a new database
        DbHelper.ConnectionString = $"{baseConnectionString};Database=master;";
        DbHelper.Exec($"CREATE DATABASE {databaseName}");
        Console.WriteLine("Database created successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        // Switch back to the newly created database
        DbHelper.ConnectionString = $"{baseConnectionString};Database={databaseName};";
    }
}
```

#### Create Table

This method creates a table named `Persons` with columns for ID, Name, and Age. The ID column is set as the primary key.

```csharp
static void CreateTable()
{
    try
    {
        // SQL query to create the Persons table
        string createTableQuery = """
            CREATE TABLE Persons (
                ID INT PRIMARY KEY,
                Name NVARCHAR(50),
                Age INT);
            """;
        DbHelper.Exec(createTableQuery);
        Console.WriteLine("Table 'Persons' created successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
```

#### Insert Random Record

This method inserts a random record into the `Persons` table. It generates a random person and executes an `INSERT` command.

```csharp
static void InsertRandomRecord()
{
    try
    {
        // Generate a random person
        var person = RandomPerson();

        // SQL query to insert the person into the Persons table
        string insertRecordQuery = "INSERT INTO Persons (ID, Name, Age) VALUES (@ID, @Name, @Age)";
        DbHelper.Exec(insertRecordQuery, person);
        Console.WriteLine($"ID: {person.ID}, Name: {person.Name}, Age: {person.Age} inserted successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```

#### Read First Record

This method reads the first record from the `Persons` table and displays it. If the table is empty, it notifies the user.

```csharp
static void Read1stRecord()
{
    try
    {
        // SQL query to select the first record from the Persons table
        string selectQuery = "SELECT TOP 1 * FROM Persons";
        var person = DbHelper.Read<Person>(selectQuery).FirstOrDefault();
        if (person != null)
        {
            Console.WriteLine($"ID: {person.ID}, Name: {person.Name}, Age: {person.Age}");
        }
        else
        {
            Console.WriteLine("Persons table is empty.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```

#### Read All Records

This method reads all records from the `Persons` table and displays them. It iterates through the list of persons and prints each one.

```csharp
static void ReadAllRecords()
{
    try
    {
        // SQL query to select all records from the Persons table
        string selectAllQuery = "SELECT * FROM Persons";
        var persons = DbHelper.Read<Person>(selectAllQuery);
        foreach (Person person in persons)
        {
            Console.WriteLine($"ID:{person.ID}, Name:{person.Name}, Age:{person.Age}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```

#### Count Records

This method counts the number of records in the `Persons` table and displays the count.

```csharp
static void CountRecords()
{
    try
    {
        // SQL query to count the number of records in the Persons table
        string countQuery = "SELECT COUNT(*) FROM Persons";
        int count = DbHelper.Exec<int>(countQuery);
        Console.WriteLine($"Number of records: {count}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```

#### Update First Record

This method updates the first record in the `Persons` table with new random values. If the table is empty, it notifies the user.

```csharp
static void Update1stRecord()
{
    try
    {
        // Generate a random person
        var person = RandomPerson();

        // SQL query to update the first record in the Persons table
        string updateQuery = $"UPDATE TOP (1) Persons SET Name = @Name, Age = @Age";
        var count = DbHelper.Exec(updateQuery, person);
        if (count > 0)
        {
            Console.WriteLine($"Name: {person.Name}, Age: {person.Age} updated successfully");
        }
        else
        {
            Console.WriteLine("Can't update 1st record. No records in table.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```

#### Delete First Record

This method deletes the first record from the `Persons` table. If the table is empty, it notifies the user.

```csharp
static void Delete1stRecord()
{
    try
    {
        // SQL query to delete the first record from the Persons table
        string deleteQuery = $"DELETE TOP (1) FROM Persons";
        var count = DbHelper.Exec(deleteQuery);
        if (count > 0)
        {
            Console.WriteLine("1st record was deleted.");
        }
        else
        {
            Console.WriteLine("Can't delete 1st record. No records in table.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
```

#### Delete Database

This method deletes the database. It first connects to the master database, executes the `DROP DATABASE` command, and then switches back to the base connection string.

```csharp
static void DeleteDatabase(string baseConnectionString, string databaseName)
{
    try
    {
        DbHelper.ConnectionString = $"{baseConnectionString};Database=master;";
        DbHelper.Exec($"DROP DATABASE IF EXISTS {databaseName};");
        Console.WriteLine("Database deleted successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        DbHelper.ConnectionString = baseConnectionString;
    }
}
```

### Helper Methods

#### Generate Random Person

```csharp
static Person RandomPerson()
{
    var random = new Random();
    var persons = new List<Person>
    {
        new Person { ID = 1, Name = "John Doe", Age = 30 },
        new Person { ID = 2, Name = "Jane Smith", Age = 25 },
        new Person { ID = 3, Name = "Sam Brown", Age = 40 },
        new Person { ID = 4, Name = "Lisa White", Age = 35 },
        new Person { ID = 5, Name = "Michael Johnson", Age = 28 },
        new Person { ID = 6, Name = "Emily Davis", Age = 22 },
        new Person { ID = 7, Name = "David Wilson", Age = 45 },
        new Person { ID = 8, Name = "Sarah Miller", Age = 32 },
        new Person { ID = 9, Name = "James Taylor", Age = 38 },
        new Person { ID = 10, Name = "Laura Anderson", Age = 27 }
    };
    return persons[random.Next(persons.Count)];
}
```

#### Person Class

```csharp
class Person
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
```
