using System.Data;
using static UkrGuru.Sql.Tests.GlobalTests;

namespace UkrGuru.Sql.Tests;

public partial class DbHelperTests
{
    public DbHelperTests() => DbHelper.ConnectionString = TestConnectionString;

    public static readonly TheoryData<byte[]> GetTestBytes = new() { Array.Empty<byte>(), TestBytes1k, TestBytes5k, TestBytes55k };

    public static readonly TheoryData<char[]> GetTestChars = new() { Array.Empty<char>(), TestChars1k, TestChars5k, TestChars55k };

    public static readonly TheoryData<string> GetTestStrings = new() { string.Empty, TestString1k, TestString5k, TestString55k };

    [Fact]
    public void CanExec_Null()
    {
        Assert.Null(DbHelper.Exec<bool?>("SELECT NULL;"));
        Assert.Null(DbHelper.Exec<bool?>("SELECT @Data;", DBNull.Value));
        Assert.Equal(-1, DbHelper.Exec("DECLARE @Num0 int = 0;"));
    }

    [Fact]
    public void CanExec_Boolean()
    {
        Assert.True(DbHelper.Exec<bool>("SELECT @Data", true));
        Assert.False(DbHelper.Exec<bool>("SELECT @Data", false));
    }

    [Fact]
    public void CanExec_Numeric()
    {
        object? value;

        value = byte.MinValue;
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", value));

        value = byte.MaxValue;
        Assert.Equal(value, DbHelper.Exec<byte>("SELECT @Data", value));

        value = short.MaxValue;
        Assert.Equal(value, DbHelper.Exec<short>("SELECT @Data", value));

        value = int.MaxValue;
        Assert.Equal(value, DbHelper.Exec<int>("SELECT @Data", value));

        value = long.MaxValue;
        Assert.Equal(value, DbHelper.Exec<long>("SELECT @Data", value));

        value = decimal.MaxValue;
        Assert.Equal(value, DbHelper.Exec<decimal>("SELECT @Data", value));

        value = float.MaxValue;
        Assert.Equal(value, DbHelper.Exec<float>("SELECT @Data", value));

        value = double.MaxValue;
        Assert.Equal(value, DbHelper.Exec<double>("SELECT @Data", value));
    }

    [Fact]
    public void CanExec_DateTime()
    {
        object? value;

        value = DateOnly.MaxValue;
        Assert.Equal(value, DbHelper.Exec<DateOnly>("SELECT @Data", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59);
        Assert.Equal(value, DbHelper.Exec<DateTime>("SELECT @Data", value));

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
        object? value;

        value = Guid.NewGuid();
        Assert.Equal(value, DbHelper.Exec<Guid>("SELECT @Data", value));

        value = 'V';
        Assert.Equal(value, DbHelper.Exec<char>("SELECT @Data", value));

        value = string.Empty;
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", value));

        value = "A V";
        Assert.Equal(value, DbHelper.Exec<string>("SELECT @Data", value));

        value = UserType.User;
        Assert.Equal(value, DbHelper.Exec<UserType?>("SELECT @Data", value));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public void CanExec_Bytes(byte[] bytes)
        => Assert.Equal(bytes, DbHelper.Exec<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestChars))]
    public void CanExec_Chars(char[] chars)
        => Assert.Equal(chars, DbHelper.Exec<char[]?>("SELECT @Data", chars));

    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public void CanExec_String(string str)
        => Assert.Equal(str, DbHelper.Exec<string?>("SELECT @Data", str));

    [Fact]
    public void CanExec_Record()
    {
        Assert.Equal("John", DbHelper.Exec<string?>("SELECT @Name;", new { Name = "John" }));

        var rec1 = DbHelper.Read<Person>("SELECT 1 Id, 'John' Name").FirstOrDefault();
        Assert.NotNull(rec1);
        Assert.Equal(1, rec1.Id);
        Assert.Equal("John", rec1.Name);

        var recs = DbHelper.Read<Person>("SELECT 1 Id, 'John' Name UNION ALL SELECT 2 Id, 'Mike' Name").ToList();
        Assert.NotNull(recs);
        Assert.Equal(2, recs.Count);
        Assert.NotNull(recs[0]);
        Assert.Equal(1, recs[0]?.Id);
        Assert.Equal("John", recs[0]?.Name);
        Assert.NotNull(recs[1]);
        Assert.Equal(2, recs[1]?.Id);
        Assert.Equal("Mike", recs[1]?.Name);
    }

    [Fact]
    public void CanExec_Crud()
    {
        var item1 = new { Name = "DbHelperName1" };

        var id = DbHelper.Exec<int?>(@"INSERT INTO TestItems (Name) OUTPUT INSERTED.Id VALUES (@Name)", item1);

        Assert.NotNull(id);

        var item2 = DbHelper.Read<Person?>(@"SELECT Id, Name FROM TestItems WHERE Id = @Data", id).FirstOrDefault();

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "DbHelperName2";

        DbHelper.Exec(@"UPDATE TestItems SET Name = @Name WHERE Id = @Id", new { item2.Id, item2.Name });

        var item3 = DbHelper.Read<Person>(@"SELECT Id, Name FROM TestItems WHERE Id = @Data", id).FirstOrDefault();

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        DbHelper.Exec(@"DELETE TestItems WHERE Id = @Data", id);

        var item4 = DbHelper.Read<Person?>(@"SELECT Id, Name FROM TestItems WHERE Id = @Data", id).FirstOrDefault();

        Assert.Null(item4);
    }

    [Fact]
    public async Task CreateCommandAsync_DisposesConnectionWithCommand()
    {
        // Arrange
        var sql = "SELECT @Data;";
        var data = new { Data = 123 };

        // Act
        var cmd = await DbHelper.CreateCommandAsync(sql, data, null);

        // Assert: Command created
        Assert.NotNull(cmd);
        Assert.Equal(sql, cmd.CommandText);

        // Assert: Connection is open
        var conn = cmd.Connection;
        Assert.NotNull(conn);
        Assert.Equal(ConnectionState.Open, conn.State);

        // Assert: Parameters populated
        Assert.True(cmd.Parameters.Contains("@Data"));
        Assert.Equal(123, cmd.Parameters["@Data"].Value);

        // Act: Dispose command (should trigger connection.Dispose via event)
        cmd.Dispose();

        // Assert: Connection closed
        Assert.Equal(ConnectionState.Closed, conn.State);
    }
}
