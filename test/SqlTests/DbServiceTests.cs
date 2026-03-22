// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static UkrGuru.Sql.Tests.GlobalTests;

namespace UkrGuru.Sql.Tests;

public partial class DbServiceTests
{
    private readonly IDbService _db;

    public DbServiceTests() => _db = new DbService(TestConnectionString);

    public static readonly TheoryData<byte[]> GetTestBytes = new() { Array.Empty<byte>(), TestBytes1k, TestBytes5k, TestBytes55k };

    public static readonly TheoryData<char[]> GetTestChars = new() { Array.Empty<char>(), TestChars1k, TestChars5k, TestChars55k };

    public static readonly TheoryData<string> GetTestStrings = new() { string.Empty, TestString1k, TestString5k, TestString55k };

    [Fact]
    public async Task CanExecAsync_Null()
    {
        Assert.Null(await _db.ExecAsync<bool?>("SELECT NULL;"));
        Assert.Null(await _db.ExecAsync<bool?>("SELECT @Data;", DBNull.Value));
        Assert.Equal(-1, await _db.ExecAsync("DECLARE @Num0 int = 0;"));
    }

    [Fact]
    public async Task CanExecAsync_Boolean()
    {
        Assert.True(await _db.ExecAsync<bool>("SELECT @Data", true));
        Assert.False(await _db.ExecAsync<bool>("SELECT @Data", false));
    }

    [Fact]
    public async Task CanExecAsync_Numeric()
    {
        object? value;

        value = byte.MinValue;
        Assert.Equal(value, await _db.ExecAsync<byte>("SELECT @Data", value));

        value = byte.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<byte>("SELECT @Data", value));

        value = short.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<short>("SELECT @Data", value));

        value = int.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<int>("SELECT @Data", value));

        value = long.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<long>("SELECT @Data", value));

        value = decimal.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<decimal>("SELECT @Data", value));

        value = float.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<float>("SELECT @Data", value));

        value = double.MaxValue;
        Assert.Equal(value, await _db.ExecAsync<double>("SELECT @Data", value));
    }

    [Fact]
    public async Task CanExecAsync_DateTime()
    {
        object? value;

        value = DateOnly.MaxValue;

        Assert.Equal(value, await _db.ExecAsync<DateOnly>("SELECT @Data", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59);
        Assert.Equal(value, await _db.ExecAsync<DateTime>("SELECT @Data", value));

        value = new DateTimeOffset((DateTime)value);
        Assert.Equal(value, await _db.ExecAsync<DateTimeOffset>("SELECT @Data", value));

        value = new TimeOnly(23, 59, 59);
        Assert.Equal(value, await _db.ExecAsync<TimeOnly>("SELECT @Data", value));

        value = new TimeSpan(0, 23, 59, 59, 999);
        Assert.Equal(value, await _db.ExecAsync<TimeSpan>("SELECT @Data", value));
    }

    [Fact]
    public async Task CanExecAsync_Other()
    {
        object? value;

        value = Guid.NewGuid();
        Assert.Equal(value, await _db.ExecAsync<Guid>("SELECT @Data", value));

        value = 'V';
        Assert.Equal(value, await _db.ExecAsync<char>("SELECT @Data", value));

        value = string.Empty;
        Assert.Equal(value, await _db.ExecAsync<string>("SELECT @Data", value));

        value = "A V";
        Assert.Equal(value, await _db.ExecAsync<string>("SELECT @Data", value));

        value = UserType.User;
        Assert.Equal(value, await _db.ExecAsync<UserType?>("SELECT @Data", value));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public async Task CanExecAsync_Bytes(byte[] bytes)
        => Assert.Equal(bytes, await _db.ExecAsync<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestChars))]
    public async Task CanExecAsync_Chars(char[] chars)
        => Assert.Equal(chars, await _db.ExecAsync<char[]?>("SELECT @Data", chars));

    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public async Task CanExecAsync_String(string str)
        => Assert.Equal(str, await _db.ExecAsync<string?>("SELECT @Data", str));

    [Fact]
    public async Task CanExecAsync_Record()
    {
        Assert.Equal("John", await _db.ExecAsync<string?>("SELECT @Name;", new { Name = "John" }));

        var rec1 = (await _db.ReadAsync<Person>("SELECT 1 Id, 'John' Name")).FirstOrDefault();
        Assert.NotNull(rec1);
        Assert.Equal(1, rec1.Id);
        Assert.Equal("John", rec1.Name);

        var recs = (await _db.ReadAsync<Person>("SELECT 1 Id, 'John' Name UNION ALL SELECT 2 Id, 'Mike' Name")).ToList();
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
    public async Task CanExecAsync_Crud()
    {
        var item1 = new { Name = "_dbName1" };

        var id = await _db.ExecAsync<int?>(@"INSERT INTO TestItems (Name) OUTPUT INSERTED.Id VALUES (@Name)", item1);

        Assert.NotNull(id);

        var item2 = (await _db.ReadAsync<Person?>(@"SELECT Id, Name FROM TestItems WHERE Id = @Data", id)).FirstOrDefault();

        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);

        item2.Name = "_dbName2";

        await _db.ExecAsync(@"UPDATE TestItems SET Name = @Name WHERE Id = @Id", new { item2.Id, item2.Name });

        var item3 = (await _db.ReadAsync<Person>(@"SELECT Id, Name FROM TestItems WHERE Id = @Data", id)).FirstOrDefault();

        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);

        await _db.ExecAsync(@"DELETE TestItems WHERE Id = @Data", id);

        var item4 = (await _db.ReadAsync<Person?>(@"SELECT Id, Name FROM TestItems WHERE Id = @Data ", id)).FirstOrDefault();

        Assert.Null(item4);
    }
}
