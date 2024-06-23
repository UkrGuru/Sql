using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using System.Xml;
using UkrGuru.Sql;
using static SqlTests.GlobalTests;
using System.Data;

namespace SqlTests;

public partial class DbHelperTests
{
    public DbHelperTests()
    {
        DbHelper.ConnectionString = ConnectionString;
    }

    public static readonly TheoryData<byte[]> GetTestBytes = new() { Array.Empty<byte>(), TestBytes1k, TestBytes5k, TestBytes55k };

    public static readonly TheoryData<char[]> GetTestChars = new() { Array.Empty<char>(), TestChars1k, TestChars5k, TestChars55k };

    public static readonly TheoryData<string> GetTestStrings = new() { string.Empty, TestString1k, TestString5k, TestString55k };

    [Fact]
    public void CanExec()
    {
        DbHelper.Exec<int>("TRUNCATE TABLE InputVar", behavior: CommandBehavior.Default);

        Assert.Equal(1, DbHelper.Exec<int>("INSERT INTO InputVar VALUES (0, @Data)", DBNull.Value, behavior: CommandBehavior.Default));

        Assert.Equal(1, DbHelper.Exec<int>("INSERT INTO InputVar VALUES (1, @Data)", 1, behavior: CommandBehavior.Default));

        Assert.Equal(1, DbHelper.Exec<int>("INSERT INTO InputVar VALUES (2, JSON_VALUE(@Data, '$.value'))", 
            new { value = 2 }, behavior: CommandBehavior.Default));

        Assert.Equal(2, DbHelper.Exec<int>("""INSERT INTO InputVar SELECT * FROM OPENJSON(@Data) WITH (ID int, Data int)""",
            new[] { new { ID = 3, Data = 3 }, new { ID = 4, Data = 4 } }, behavior: CommandBehavior.Default));

        Assert.Equal(1, DbHelper.Exec<int>("INSERT INTO InputVar (ID, Data) VALUES (10, @MyName)", 
            new SqlParameter("@MyName", 10), behavior: CommandBehavior.Default));

        Assert.Equal(1, DbHelper.Exec<int>("INSERT INTO InputVar (ID, Data) VALUES (@ID, @Data)", 
            SqlParams.Parse(new { ID = 11, Data = 11 }), behavior: CommandBehavior.Default));
    }

    [Fact]
    public void CanRead()
    {
        DbHelper.Exec<int>("TRUNCATE TABLE [dbo].[Events]", behavior: CommandBehavior.Default);

        Assert.Equal(2, DbHelper.Exec<int>("""
            SET IDENTITY_INSERT [dbo].[Events] ON;
            INSERT [dbo].[Events] ([Id], [Name], [Date], [Venue]) VALUES (1, N'Event1', CAST(N'2000-01-01T00:00:00' AS SmallDateTime), N'London');
            INSERT [dbo].[Events] ([Id], [Name], [Date], [Venue]) VALUES (2, N'Event2', CAST(N'2000-01-02T00:00:00' AS SmallDateTime), N'New York');
            SET IDENTITY_INSERT [dbo].[Events] OFF;
            """, behavior: CommandBehavior.Default));

        var event1 = DbHelper.Read<Event>("SELECT * FROM [dbo].[Events] WHERE Id = 1").FirstOrDefault();
        Assert.NotNull(event1);
        Assert.Equal(1, event1.Id);
        Assert.Equal("Event1", event1.Name);

        var event2 = DbHelper.Read<Event>("SELECT * FROM [dbo].[Events] WHERE Id = 2").FirstOrDefault();
        Assert.NotNull(event2);
        Assert.Equal(2, event2.Id);
        Assert.Equal("Event2", event2.Name);

        var events = DbHelper.Read<Event>("SELECT * FROM [dbo].[Events]").ToList();
        Assert.NotNull(events);
        Assert.Equal(2, events.Count);
        Assert.Equal(event1.Id, events[0]?.Id);
        Assert.Equal(event1.Name, events[0]?.Name);
        Assert.Equal(event2.Id, events[1]?.Id);
        Assert.Equal(event2.Name, events[1]?.Name);
    }


    [Fact]
    public void CanExec_Null()
    {
        Assert.Equal(-1, DbHelper.Exec<int>("DECLARE @Num0 int = 0", behavior: CommandBehavior.Default));
        Assert.Equal(1, DbHelper.Exec<int>("DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1)", behavior: CommandBehavior.Default));

        Assert.Null(DbHelper.Exec<bool?>("DECLARE @Num0 int = 0"));
        Assert.Null(DbHelper.Exec<bool?>("SELECT NULL"));

        Assert.Null(DbHelper.Exec<bool?>("SELECT @Data", DBNull.Value));

        Assert.Null(DbHelper.Exec<byte?>("SELECT @Data", SqlByte.Null));
        Assert.Null(DbHelper.Exec<short?>("SELECT @Data", SqlInt16.Null));
        Assert.Null(DbHelper.Exec<int?>("SELECT @Data", SqlInt32.Null));
        Assert.Null(DbHelper.Exec<long?>("SELECT @Data", SqlInt64.Null));
        Assert.Null(DbHelper.Exec<float?>("SELECT @Data", SqlSingle.Null));
        Assert.Null(DbHelper.Exec<double?>("SELECT @Data", SqlDouble.Null));
        Assert.Null(DbHelper.Exec<decimal?>("SELECT @Data", SqlDecimal.Null));
        Assert.Null(DbHelper.Exec<decimal?>("SELECT @Data", SqlMoney.Null));

        Assert.Null(DbHelper.Exec<DateTime?>("SELECT @Data", SqlDateTime.Null));

        Assert.Null(DbHelper.Exec<bool?>("SELECT @Data", SqlBoolean.Null));
        Assert.Null(DbHelper.Exec<Guid?>("SELECT @Data", SqlGuid.Null));

        Assert.Null(DbHelper.Exec<byte[]?>("SELECT @Data", SqlBinary.Null));
        Assert.Null(DbHelper.Exec<byte[]?>("SELECT @Data", SqlBytes.Null));

        Assert.Null(DbHelper.Exec<char[]?>("SELECT @Data", SqlChars.Null));
        Assert.Null(DbHelper.Exec<string?>("SELECT @Data", SqlString.Null));

        Assert.Null(DbHelper.Exec<string?>("SELECT @Data", SqlXml.Null));
    }

    [Fact]
    public void CanExec_Boolean()
    {
        Assert.True(DbHelper.Exec<bool>("SELECT @Data", true));
        Assert.True(DbHelper.Exec<bool>("SELECT @Data", SqlBoolean.True));

        Assert.False(DbHelper.Exec<bool>("SELECT @Data", false));
        Assert.False(DbHelper.Exec<bool>("SELECT @Data", SqlBoolean.False));
    }

    [Fact]
    public void CanExec_Numeric()
    {
        object? value, sqlValue;

        value = byte.MinValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", sqlValue));

        value = byte.MaxValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", sqlValue));

        value = short.MaxValue; sqlValue = new SqlInt16((short)value);
        Assert.Equal(value, DbHelper.Exec<short>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<short>("SELECT @Data", sqlValue));

        value = int.MaxValue; sqlValue = new SqlInt32((int)value);
        Assert.Equal(value, DbHelper.Exec<int>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<int>("SELECT @Data", sqlValue));

        value = long.MaxValue; sqlValue = new SqlInt64((long)value);
        Assert.Equal(value, DbHelper.Exec<long>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<long>("SELECT @Data", sqlValue));

        value = decimal.MaxValue; sqlValue = new SqlDecimal((decimal)value);
        Assert.Equal(value, DbHelper.Exec<decimal>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<decimal>("SELECT @Data", sqlValue));

        value = float.MaxValue; sqlValue = new SqlSingle((float)value);
        Assert.Equal(value, DbHelper.Exec<float>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<float>("SELECT @Data", sqlValue));

        value = double.MaxValue; sqlValue = new SqlDouble((double)value);
        Assert.Equal(value, DbHelper.Exec<double>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<double>("SELECT @Data", sqlValue));

        value = 45m; sqlValue = new SqlMoney((decimal)value);
        Assert.Equal(value, DbHelper.Exec<decimal>("SELECT @Data", sqlValue));
    }

    [Fact]
    public void CanExec_DateTime()
    {
        object? value, sqlValue;

        value = DateOnly.MaxValue;
        Assert.Equal(value, DbHelper.Exec<DateOnly>("SELECT @Data", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59); sqlValue = new SqlDateTime((DateTime)value);
        Assert.Equal(value, DbHelper.Exec<DateTime>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<DateTime>("SELECT @Data", sqlValue));

        value = new DateTimeOffset((DateTime)value);
        Assert.Equal(value, DbHelper.Exec<DateTimeOffset>("SELECT @Data", value));

        value = new TimeOnly(23, 59, 59);
        Assert.Equal(value, DbHelper.Exec<TimeOnly>("SELECT @Data", value));

        value = new TimeSpan(0, 23, 59, 59, 999);
        Assert.Equal(value, DbHelper.Exec<TimeSpan>("SELECT @Data", value));
    }

    [Fact]
    public void CanExec_Other()
    {
        object? value, sqlValue;

        value = Guid.NewGuid(); sqlValue = new SqlGuid((Guid)value);
        Assert.Equal(value, DbHelper.Exec<Guid>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<Guid>("SELECT @Data", sqlValue));

        value = 'V';
        Assert.Equal(value, DbHelper.Exec<char>("SELECT @Data", value));

        value = string.Empty; sqlValue = new SqlString((string)value);
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", sqlValue));

        value = "A V"; sqlValue = new SqlString((string)value);
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", value));
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", sqlValue));

        value = UserType.User;
        Assert.Equal(value, DbHelper.Exec<UserType?>("SELECT @Data", value));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public void CanExec_Bytes(byte[] bytes)
        => Assert.Equal(bytes, DbHelper.Exec<byte[]?>("SELECT @Data", bytes, 
            behavior: CommandBehavior.CloseConnection));

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public void CanExec_SqlBinary(byte[] bytes)
    {
        var sqlValue = new SqlBinary(bytes);
        Assert.Equal(sqlValue.Value, (DbHelper.Exec<SqlBinary>("SELECT @Data", sqlValue, 
            behavior: CommandBehavior.CloseConnection)).Value);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public void CanExec_SqlBytes(byte[] bytes)
    {
        var sqlValue = new SqlBytes(bytes);
        Assert.Equal(sqlValue.Value, (DbHelper.Exec<SqlBytes>("SELECT @Data", sqlValue, 
            behavior: CommandBehavior.CloseConnection))!.Value);
    }

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public void CanExec_Stream(byte[] bytes)
    {
        using var msIn = new MemoryStream(bytes);
        using var stream = DbHelper.Exec<Stream>("SELECT @Data", msIn, 
            behavior: CommandBehavior.CloseConnection);

        Assert.NotNull(stream);
        Assert.Equal(bytes, Stream2Bytes(stream));

        static byte[] Stream2Bytes(Stream input)
        {
            MemoryStream ms = new();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public void CanExec_Stream_IO(byte[] bytes)
    {
        int? id;

        using (var msIn = new MemoryStream(bytes))
        {
            id = DbHelper.Exec<int>("INSERT INTO [BinaryStreams] (bindata) VALUES (@Data); SELECT SCOPE_IDENTITY();", msIn);
            Assert.NotNull(id);
        }

        using (var stream = DbHelper.Exec<Stream>("SELECT bindata FROM[BinaryStreams] WHERE id = @Data", id,
            behavior: CommandBehavior.CloseConnection))
        {
            Assert.NotNull(stream);
            Assert.Equal(bytes, Stream2Bytes(stream));
        }

        static byte[] Stream2Bytes(Stream input)
        {
            MemoryStream ms = new();
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    [Theory]
    [MemberData(nameof(GetTestChars))]
    public void CanExec_Chars(char[] chars)
        => Assert.Equal(chars, DbHelper.Exec<char[]?>("SELECT @Data", chars, 
            behavior: CommandBehavior.CloseConnection));


    [Theory]
    [MemberData(nameof(GetTestChars))]
    public void CanExec_SqlChars(char[] chars)
    {
        var sqlValue = new SqlChars(chars);
        Assert.Equal(sqlValue.Value, (DbHelper.Exec<SqlChars>("SELECT @Data", sqlValue, 
            behavior: CommandBehavior.CloseConnection))!.Value);
    }

    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public void CanExec_String(string str)
        => Assert.Equal(str, DbHelper.Exec<string?>("SELECT @Data", str));

    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public void CanExec_TextReader(string text)
    {
        using TextReader readerSource = new StringReader(text);
        using var readerResult = DbHelper.Exec<TextReader>("SELECT @Data", readerSource, 
            behavior: CommandBehavior.CloseConnection);

        Assert.NotNull(readerResult);
        Assert.Equal(text, readerResult.ReadToEnd());
    }


    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public void CanExec_TextReader_IO(string text)
    {
        int? id;

        using (TextReader readerSource = new StringReader(text))
        {
            id = DbHelper.Exec<int>("INSERT INTO [TextStreams] (textdata) VALUES (@Data); SELECT SCOPE_IDENTITY();", readerSource);
            Assert.NotNull(id);
        }

        using (var readerResult = DbHelper.Exec<TextReader>("SELECT textdata FROM [TextStreams] WHERE id = @Data", id, 
            behavior: CommandBehavior.CloseConnection))
        {
            Assert.NotNull(readerResult);
            Assert.Equal(text, readerResult.ReadToEnd());
        }
    }

    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public void CanExec_XmlReader(string text)
    {
        var value = string.IsNullOrEmpty(text) ? "<value />" : new XElement("value", text).ToString();

        using var readerIn = XmlReader.Create(new StringReader(value));

        using var readerOut = DbHelper.Exec<XmlReader>("SELECT @Data", new SqlXml(readerIn), 
            behavior: CommandBehavior.CloseConnection);

        Assert.NotNull(readerOut);

        readerOut.Read();

        Assert.Equal(value, readerOut.ReadOuterXml());
    }

    [Fact]
    public void CanExec_Record()
    {
        Assert.Equal("John", DbHelper.Exec<string?>("ProcObj", new { Name = "John" }));

        var rec1 = DbHelper.Exec<JsonObject>("ProcObj1");
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);

        var recs = DbHelper.Exec<List<JsonObject>>("ProcObj2");
        Assert.NotNull(recs);
        Assert.Equal(2, recs.Count);
        Assert.Equal(1, (int?)recs[0]["Id"]);
        Assert.Equal("John", (string?)recs[0]["Name"]);
        Assert.Equal(2, (int?)recs[1]["Id"]);
        Assert.Equal("Mike", (string?)recs[1]["Name"]);
    }

    [Fact]
    public void CanExec_Crud()
    {
        var item1 = new { Name = "DbHelperName1" };

        var id = DbHelper.Exec<decimal?>(@"
INSERT INTO TestItems 
SELECT * FROM OPENJSON(@Data) 
WITH (Name nvarchar(50))

SELECT SCOPE_IDENTITY()
", item1);

        Assert.NotNull(id);

        var item2 = DbHelper.Exec<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "DbHelperName2";

        DbHelper.Exec<int>(@"
UPDATE TestItems
SET Name = D.Name
FROM OPENJSON(@Data) 
WITH (Id int, Name nvarchar(50)) D
WHERE TestItems.Id = D.Id
", item2, behavior: CommandBehavior.Default);

        var item3 = DbHelper.Exec<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        DbHelper.Exec<int>(@"
DELETE TestItems
WHERE Id = @Data
", id, behavior: CommandBehavior.Default);

        var item4 = DbHelper.Exec<Region?>(@"
SELECT *
FROM TestItems
WHERE Id = @Data
FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
", id);

        Assert.Null(item4);
    }

}