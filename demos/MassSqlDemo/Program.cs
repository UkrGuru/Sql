using UkrGuru.Sql;

Utils.InitDb();

int N = 100 * 1024; 
List<Student>? students = new();
DateTime started = DateTime.Now;

// MASS INSERT
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

// MASS UPDATE
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

// MASS DELETE
var sql_delete = """
    DELETE S 
    FROM Students S 
    JOIN OPENJSON(@Data) AS D ON S.ID = D.value;
    """;

started = DateTime.Now;

await DbHelper.ExecAsync(sql_delete,
    students.Where(x => x.Grade < 1).Select(c => c.ID ).ToJson());

Console.WriteLine($"Deleted {(N / 5) / 1024}K - {DateTime.Now.Subtract(started)}");

class Student
{
    public int ID { get; set; }
    public string? Name { get; set; }
    public char? Class { get; set; }
    public byte? Grade { get; set; }
}

class Utils {
    public static void InitDb()
    {
        var BaseConnectionString = "Data Source=(Local);Initial Catalog={DbName};Integrated Security=True;Trust Server Certificate=True;";
        var DatabaseName = "UkrGuruSqlMass";
        try
        {
            DbHelper.ConnectionString = $"{BaseConnectionString};Database=master;";

            DbHelper.Exec($"""
        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{DatabaseName}')
        BEGIN
            CREATE DATABASE [{DatabaseName}];
        END
        """);

            Console.WriteLine("Database created successfully.");

            DbHelper.ConnectionString = $"{BaseConnectionString};Database={DatabaseName};";

            DbHelper.Exec("""
        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Students]') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[Students](
            	[ID] [int] NOT NULL,
            	[Name] [varchar](50) NULL,
            	[Class] [char](1) NULL,
            	[Grade] [tinyint] NULL,
            	CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED 
            (
            	[ID] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        ELSE
        BEGIN
            TRUNCATE TABLE [dbo].[Students]
        END
        
        IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Students]') AND type in (N'U'))
        BEGIN
            CREATE TABLE [dbo].[Students](
            	[ID] [int] NOT NULL,
            	[Name] [varchar](50) NULL,
            	[Class] [char](1) NULL,
            	[Grade] [tinyint] NULL,
            	CONSTRAINT [PK_Students] PRIMARY KEY CLUSTERED 
            (
            	[ID] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
            ) ON [PRIMARY]
        END
        ELSE
        BEGIN
            TRUNCATE TABLE [dbo].[Students]
        END
        
        """);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            DbHelper.ConnectionString = $"{BaseConnectionString};Database={DatabaseName};";
        }
    }
}
