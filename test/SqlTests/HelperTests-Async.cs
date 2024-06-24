// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static SqlTests.GlobalTests;
using System.Data.SqlTypes;
using System.Text.Json.Nodes;
using UkrGuru.Sql;

namespace SqlTests;

public partial class HelperTests
{
    [Fact]
    public async Task CanExecAsync_Null()
    {
        await Helper.ExecAsync("DECLARE @Num0 int = 0;");
        Assert.Equal(1, await Helper.ExecAsync("DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1); SELECT @@ROWCOUNT;"));

        Assert.Null(await Helper.ExecAsync<bool?>("DECLARE @Num0 int = 0"));
        Assert.Null(await Helper.ExecAsync<bool?>("SELECT NULL"));

        Assert.Null(await Helper.ExecAsync<bool?>("SELECT @Data", DBNull.Value));

        Assert.Null(await Helper.ExecAsync<byte?>("SELECT @Data", SqlByte.Null));
        Assert.Null(await Helper.ExecAsync<short?>("SELECT @Data", SqlInt16.Null));
        Assert.Null(await Helper.ExecAsync<int?>("SELECT @Data", SqlInt32.Null));
        Assert.Null(await Helper.ExecAsync<long?>("SELECT @Data", SqlInt64.Null));
        Assert.Null(await Helper.ExecAsync<float?>("SELECT @Data", SqlSingle.Null));
        Assert.Null(await Helper.ExecAsync<double?>("SELECT @Data", SqlDouble.Null));
        Assert.Null(await Helper.ExecAsync<decimal?>("SELECT @Data", SqlDecimal.Null));
        Assert.Null(await Helper.ExecAsync<decimal?>("SELECT @Data", SqlMoney.Null));

        Assert.Null(await Helper.ExecAsync<DateTime?>("SELECT @Data", SqlDateTime.Null));

        Assert.Null(await Helper.ExecAsync<bool?>("SELECT @Data", SqlBoolean.Null));
        Assert.Null(await Helper.ExecAsync<Guid?>("SELECT @Data", SqlGuid.Null));

        Assert.Null(await Helper.ExecAsync<byte[]?>("SELECT @Data", SqlBinary.Null));
        Assert.Null(await Helper.ExecAsync<byte[]?>("SELECT @Data", SqlBytes.Null));

        Assert.Null(await Helper.ExecAsync<char[]?>("SELECT @Data", SqlChars.Null));
        Assert.Null(await Helper.ExecAsync<string?>("SELECT @Data", SqlString.Null));

        Assert.Null(await Helper.ExecAsync<string?>("SELECT @Data", SqlXml.Null));
    }

    [Fact]
    public async Task CanExecAsync_Boolean()
    {
        Assert.True(await Helper.ExecAsync<bool>("SELECT @Data", true));
        Assert.True(await Helper.ExecAsync<bool>("SELECT @Data", SqlBoolean.True));

        Assert.False(await Helper.ExecAsync<bool>("SELECT @Data", false));
        Assert.False(await Helper.ExecAsync<bool>("SELECT @Data", SqlBoolean.False));
    }

    [Fact]
    public async Task CanExecAsync_Numeric()
    {
        object? value, sqlValue;

        value = byte.MinValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, await Helper.ExecAsync<byte>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<byte>("SELECT @Data", sqlValue));

        value = byte.MaxValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, await Helper.ExecAsync<byte>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<byte>("SELECT @Data", sqlValue));

        value = short.MaxValue; sqlValue = new SqlInt16((short)value);
        Assert.Equal(value, await Helper.ExecAsync<short>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<short>("SELECT @Data", sqlValue));

        value = int.MaxValue; sqlValue = new SqlInt32((int)value);
        Assert.Equal(value, await Helper.ExecAsync<int>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<int>("SELECT @Data", sqlValue));

        value = long.MaxValue; sqlValue = new SqlInt64((long)value);
        Assert.Equal(value, await Helper.ExecAsync<long>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<long>("SELECT @Data", sqlValue));

        value = decimal.MaxValue; sqlValue = new SqlDecimal((decimal)value);
        Assert.Equal(value, await Helper.ExecAsync<decimal>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<decimal>("SELECT @Data", sqlValue));

        value = float.MaxValue; sqlValue = new SqlSingle((float)value);
        Assert.Equal(value, await Helper.ExecAsync<float>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<float>("SELECT @Data", sqlValue));

        value = double.MaxValue; sqlValue = new SqlDouble((double)value);
        Assert.Equal(value, await Helper.ExecAsync<double>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<double>("SELECT @Data", sqlValue));

        value = 45m; sqlValue = new SqlMoney((decimal)value);
        Assert.Equal(value, await Helper.ExecAsync<decimal>("SELECT @Data", sqlValue));
    }

    [Fact]
    public async Task CanExecAsync_DateTime()
    {
        object? value, sqlValue;

        value = DateOnly.MaxValue;
        Assert.Equal(value, await Helper.ExecAsync<DateOnly>("SELECT @Data", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59); sqlValue = new SqlDateTime((DateTime)value);
        Assert.Equal(value, await Helper.ExecAsync<DateTime>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<DateTime>("SELECT @Data", sqlValue));

        value = new DateTimeOffset((DateTime)value);
        Assert.Equal(value, await Helper.ExecAsync<DateTimeOffset>("SELECT @Data", value));

        value = new TimeOnly(23, 59, 59);
        Assert.Equal(value, await Helper.ExecAsync<TimeOnly>("SELECT @Data", value));

        value = new TimeSpan(0, 23, 59, 59, 999);
        Assert.Equal(value, await Helper.ExecAsync<TimeSpan>("SELECT @Data", value));
    }

    [Fact]
    public async Task CanExecAsync_Other()
    {
        object? value, sqlValue;

        value = Guid.NewGuid(); sqlValue = new SqlGuid((Guid)value);
        Assert.Equal(value, await Helper.ExecAsync<Guid>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<Guid>("SELECT @Data", sqlValue));

        value = 'V';
        Assert.Equal(value, await Helper.ExecAsync<char>("SELECT @Data", value));

        value = string.Empty; sqlValue = new SqlString((string)value);
        Assert.Equal(value, await Helper.ExecAsync<string>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<string>("SELECT @Data", sqlValue));

        value = "A V"; sqlValue = new SqlString((string)value);
        Assert.Equal(value, await Helper.ExecAsync<string>("SELECT @Data", value));
        Assert.Equal(value, await Helper.ExecAsync<string>("SELECT @Data", sqlValue));

        value = UserType.User;
        Assert.Equal(value, await Helper.ExecAsync<UserType?>("SELECT @Data", value));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public async Task CanExecAsync_Bytes(byte[] bytes) 
        => Assert.Equal(bytes, await Helper.ExecAsync<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes))]
    public async Task CanExecAsync_SqlBinary(byte[] bytes)
    {
        var sqlValue = new SqlBinary(bytes);
        Assert.Equal(sqlValue.Value, await Helper.ExecAsync<byte[]?>("SELECT @Data", sqlValue));
    }

    [Theory]
    [MemberData(nameof(GetTestChars))]
    public async Task CanExecAsync_Chars(char[] chars)
        => Assert.Equal(chars, await Helper.ExecAsync<char[]?>("SELECT @Data", chars));

    [Theory]
    [MemberData(nameof(GetTestChars))]
    public async Task CanExecAsync_SqlChars(char[] chars)
    {
        var sqlValue = new SqlChars(chars);
        Assert.Equal(sqlValue.Value, await Helper.ExecAsync<char[]?>("SELECT @Data", sqlValue));
    }

    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public async Task CanExecAsync_String(string str)
        => Assert.Equal(str, await Helper.ExecAsync<string?>("SELECT @Data", str));

    [Theory]
    [MemberData(nameof(GetTestStrings))]
    public async Task CanExecAsync_SqlString(string str)
    {
        var sqlValue = new SqlString(str);
        Assert.Equal(sqlValue.Value, await Helper.ExecAsync<string>("SELECT @Data", sqlValue));
    }

    [Fact]
    public async Task CanExecAsync_Record()
    {
        var item1 = new Item { Id = 1, Name = "John" };

        Assert.Equal(item1.Name, await Helper.ExecAsync<string?>("SELECT JSON_VALUE(@Data, '$.Name')", item1));

        var rec1 = await Helper.ExecAsync<JsonObject>("SELECT @Data", item1);
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);

        var items = await Helper.ExecAsync<List<JsonObject>>("SELECT 1 Id, 'Name1' Name UNION ALL SELECT 2, 'Name2' FOR JSON PATH");
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        Assert.NotNull(items[0]);
        Assert.Equal(1, (int?)items[0]["Id"]);
        Assert.Equal("Name1", (string?)items[0]["Name"]);
        Assert.NotNull(items[1]);
        Assert.Equal(2, (int?)items[1]["Id"]);
        Assert.Equal("Name2", (string?)items[1]["Name"]);
    }

    [Fact]
    public async Task CanExecAsync_Crud()
    {
        var item1 = new { Name = "Name1" };

        var id = await Helper.ExecAsync<int?>("INSERT INTO TestItems (Name) OUTPUT INSERTED.Id VALUES (@Name)", new { item1.Name });
        Assert.NotNull(id);

        var item2 = (await Helper.ReadAsync<Item>("SELECT * FROM TestItems WHERE Id = @Data", id)).FirstOrDefault();
        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);
        Assert.Null(item2?.Date);

        Assert.NotNull(item2);
        item2.Name = "Name2";
        item2.Date = DateTime.Today;

        await Helper.ExecAsync<int?>("UPDATE TestItems SET Name = @Name, Date = @Date WHERE Id = @Id", new { item2.Id, item2.Name, item2.Date });

        var item3 = (await Helper.ReadAsync<Item>("SELECT * FROM TestItems WHERE Id = @Data", id)).FirstOrDefault();
        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);
        Assert.Equal(DateTime.Today, item3.Date);

        var count = await Helper.ExecAsync<int?>("DELETE TestItems WHERE Id = @Data; SELECT @@ROWCOUNT;", id);
        Assert.Equal(1, count);

        var item4 = (await Helper.ReadAsync<Item>("SELECT * FROM TestItems WHERE Id = @Data", id)).FirstOrDefault();
        Assert.Null(item4);

        var items = (await Helper.ReadAsync<Item>("SELECT 1 Id, 'Name1' Name UNION ALL SELECT 2, 'Name2'")).ToList();
        Assert.NotEmpty(items);
        Assert.NotNull(items[0]);
        Assert.Equal(1, items[0]?.Id);
        Assert.Equal("Name1", items[0]?.Name);
        Assert.NotNull(items[1]);
        Assert.Equal(2, items[1]?.Id);
        Assert.Equal("Name2", items[1]?.Name);
    }
}

//var id = await Helper.ExecAsync("INSERT INTO InputVarBinary (Data) OUTPUT INSERTED.Id VALUES (@Data)", bytes);
//var actial = await Helper.ExecAsync<byte[]?>("SELECT Data FROM InputVarBinary WHERE ID = @Data", id);
//Assert.Equal(bytes, actial);
