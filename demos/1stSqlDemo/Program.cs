using UkrGuru.Sql;

var BaseConnectionString = "Server=(localdb)\\mssqllocaldb;Integrated Security=true";
var DatabaseName = "UkrGuruSqlDemo";

DbHelper.ConnectionString = $"{BaseConnectionString};Database={DatabaseName};";

while (true)
{
    // Clear the console to keep the questions fixed at the top
    Console.Clear();

    // Place the text at the top
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

    // Output messages in a paragraph format
    Console.WriteLine("\nPlease choose an option from the list above to proceed with the corresponding database operation.");

    // Input number
    Console.Write("\nEnter your choice: ");
    Int32.TryParse(Console.ReadKey().KeyChar.ToString(), out int choice);
    Console.WriteLine("");

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

static void CreateDatabase(string baseConnectionString, string databaseName)
{
    try
    {
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
        DbHelper.ConnectionString = $"{baseConnectionString};Database={databaseName};";
    }
}

static void CreateTable()
{
    try
    {
        string createTableQuery = $"""
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

static void InsertRandomRecord()
{
    try
    {
        var person = RandomPerson();

        string insertRecordQuery = "INSERT INTO Persons (ID, Name, Age) VALUES (@ID, @Name, @Age)";

        DbHelper.Exec(insertRecordQuery, person);

        Console.WriteLine($"ID: {person.ID}, Name: {person.Name}, Age: {person.Age} inserted successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void Read1stRecord()
{
    try
    {
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

static void ReadAllRecords()
{
    try
    {
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

static void CountRecords()
{
    try
    {
        string countQuery = "SELECT COUNT(*) FROM Persons";

        int count = DbHelper.Exec<int>(countQuery);

        Console.WriteLine($"Number of records: {count}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void Update1stRecord()
{
    try
    {
        var person = RandomPerson();

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

static void Delete1stRecord()
{
    try
    {
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

class Person
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
