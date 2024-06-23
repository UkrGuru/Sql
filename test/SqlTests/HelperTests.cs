﻿// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using static SqlTests.GlobalTests;
using System.Data.SqlTypes;
using System.Text.Json.Nodes;
using UkrGuru.Sql;

namespace SqlTests;

public partial class HelperTests
{
    public HelperTests() => Helper.ConnectionString = ConnectionString;

    public static readonly TheoryData<byte[]> GetTestBytes2 = new() { Array.Empty<byte>(), TestBytes1k };

    public static readonly TheoryData<char[]> GetTestChars2 = new() { Array.Empty<char>(), TestChars1k};

    public static readonly TheoryData<string> GetTestStrings2 = new() { string.Empty, TestString1k };

    public static readonly TheoryData<byte[]> GetTestBytes4 = new() { Array.Empty<byte>(), TestBytes1k, TestBytes5k, TestBytes55k };

    public static readonly TheoryData<char[]> GetTestChars4 = new() { Array.Empty<char>(), TestChars1k, TestChars5k, TestChars55k };

    public static readonly TheoryData<string> GetTestStrings4 = new() { string.Empty, TestString1k, TestString5k, TestString55k };

    [Fact]
    public void CanExec_Null()
    {
        Assert.Equal(-1, Helper.Exec("DECLARE @Num0 int = 0"));
        Assert.Equal(1, Helper.Exec("DECLARE @Table1 TABLE(Column1 int); INSERT INTO @Table1 VALUES(1)"));

        Assert.Null(Helper.Exec<bool?>("DECLARE @Num0 int = 0"));
        Assert.Null(Helper.Exec<bool?>("SELECT NULL"));

        Assert.Null(Helper.Exec<bool?>("SELECT @Data", DBNull.Value));

        Assert.Null(Helper.Exec<byte?>("SELECT @Data", SqlByte.Null));
        Assert.Null(Helper.Exec<short?>("SELECT @Data", SqlInt16.Null));
        Assert.Null(Helper.Exec<int?>("SELECT @Data", SqlInt32.Null));
        Assert.Null(Helper.Exec<long?>("SELECT @Data", SqlInt64.Null));
        Assert.Null(Helper.Exec<float?>("SELECT @Data", SqlSingle.Null));
        Assert.Null(Helper.Exec<double?>("SELECT @Data", SqlDouble.Null));
        Assert.Null(Helper.Exec<decimal?>("SELECT @Data", SqlDecimal.Null));
        Assert.Null(Helper.Exec<decimal?>("SELECT @Data", SqlMoney.Null));

        Assert.Null(Helper.Exec<DateTime?>("SELECT @Data", SqlDateTime.Null));

        Assert.Null(Helper.Exec<bool?>("SELECT @Data", SqlBoolean.Null));
        Assert.Null(Helper.Exec<Guid?>("SELECT @Data", SqlGuid.Null));

        Assert.Null(Helper.Exec<byte[]?>("SELECT @Data", SqlBinary.Null));
        Assert.Null(Helper.Exec<byte[]?>("SELECT @Data", SqlBytes.Null));

        Assert.Null(Helper.Exec<char[]?>("SELECT @Data", SqlChars.Null));
        Assert.Null(Helper.Exec<string?>("SELECT @Data", SqlString.Null));

        Assert.Null(Helper.Exec<string?>("SELECT @Data", SqlXml.Null));
    }

    [Fact]
    public void CanExec_Boolean()
    {
        Assert.True(Helper.Exec<bool>("SELECT @Data", true));
        Assert.True(Helper.Exec<bool>("SELECT @Data", SqlBoolean.True));

        Assert.False(Helper.Exec<bool>("SELECT @Data", false));
        Assert.False(Helper.Exec<bool>("SELECT @Data", SqlBoolean.False));
    }

    [Fact]
    public void CanExec_Numeric()
    {
        object? value, sqlValue;

        value = byte.MinValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, Helper.Exec<byte>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<byte>("SELECT @Data", sqlValue));

        value = byte.MaxValue; sqlValue = new SqlByte((byte)value);
        Assert.Equal(value, Helper.Exec<byte>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<byte>("SELECT @Data", sqlValue));

        value = short.MaxValue; sqlValue = new SqlInt16((short)value);
        Assert.Equal(value, Helper.Exec<short>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<short>("SELECT @Data", sqlValue));

        value = int.MaxValue; sqlValue = new SqlInt32((int)value);
        Assert.Equal(value, Helper.Exec<int>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<int>("SELECT @Data", sqlValue));

        value = long.MaxValue; sqlValue = new SqlInt64((long)value);
        Assert.Equal(value, Helper.Exec<long>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<long>("SELECT @Data", sqlValue));

        value = decimal.MaxValue; sqlValue = new SqlDecimal((decimal)value);
        Assert.Equal(value, Helper.Exec<decimal>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<decimal>("SELECT @Data", sqlValue));

        value = float.MaxValue; sqlValue = new SqlSingle((float)value);
        Assert.Equal(value, Helper.Exec<float>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<float>("SELECT @Data", sqlValue));

        value = double.MaxValue; sqlValue = new SqlDouble((double)value);
        Assert.Equal(value, Helper.Exec<double>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<double>("SELECT @Data", sqlValue));

        value = 45m; sqlValue = new SqlMoney((decimal)value);
        Assert.Equal(value, Helper.Exec<decimal>("SELECT @Data", sqlValue));
    }

    [Fact]
    public void CanExec_DateTime()
    {
        object? value, sqlValue;

        value = DateOnly.MaxValue;
        Assert.Equal(value, Helper.Exec<DateOnly>("SELECT @Data", value));

        value = new DateTime(2000, 01, 13, 23, 59, 59); sqlValue = new SqlDateTime((DateTime)value);
        Assert.Equal(value, Helper.Exec<DateTime>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<DateTime>("SELECT @Data", sqlValue));

        value = new DateTimeOffset((DateTime)value);
        Assert.Equal(value, Helper.Exec<DateTimeOffset>("SELECT @Data", value));

        value = new TimeOnly(23, 59, 59);
        Assert.Equal(value, Helper.Exec<TimeOnly>("SELECT @Data", value));

        value = new TimeSpan(0, 23, 59, 59, 999);
        Assert.Equal(value, Helper.Exec<TimeSpan>("SELECT @Data", value));
    }

    [Fact]
    public void CanExec_Other()
    {
        object? value, sqlValue;

        value = Guid.NewGuid(); sqlValue = new SqlGuid((Guid)value);
        Assert.Equal(value, Helper.Exec<Guid>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<Guid>("SELECT @Data", sqlValue));

        value = 'V';
        Assert.Equal(value, Helper.Exec<char>("SELECT @Data", value));

        value = string.Empty; sqlValue = new SqlString((string)value);
        Assert.Equal(value, Helper.Exec<string>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<string>("SELECT @Data", sqlValue));

        value = "A V"; sqlValue = new SqlString((string)value);
        Assert.Equal(value, Helper.Exec<string>("SELECT @Data", value));
        Assert.Equal(value, Helper.Exec<string>("SELECT @Data", sqlValue));

        value = UserType.User;
        Assert.Equal(value, Helper.Exec<UserType?>("SELECT @Data", value));
    }

    [Theory]
    [MemberData(nameof(GetTestBytes2))]
    public void CanExec_Bytes(byte[] bytes)
        => Assert.Equal(bytes, Helper.Exec<byte[]?>("SELECT @Data", bytes));

    [Theory]
    [MemberData(nameof(GetTestBytes2))]
    public void CanExec_SqlBinary(byte[] bytes)
    {
        var sqlValue = new SqlBinary(bytes);
        Assert.Equal(sqlValue.Value, Helper.Exec<byte[]?>("SELECT @Data", sqlValue));
    }

    [Theory]
    [MemberData(nameof(GetTestChars2))]
    public void CanExec_Chars(char[] chars)
        => Assert.Equal(chars, Helper.Exec<char[]?>("SELECT @Data", chars));

    [Theory]
    [MemberData(nameof(GetTestChars2))]
    public void CanExec_SqlChars(char[] chars)
    {
        var sqlValue = new SqlChars(chars);
        Assert.Equal(sqlValue.Value, Helper.Exec<char[]?>("SELECT @Data", sqlValue));
    }

    [Theory]
    [MemberData(nameof(GetTestStrings2))]
    public void CanExec_String(string str)
        => Assert.Equal(str, Helper.Exec<string?>("SELECT @Data", str));

    [Theory]
    [MemberData(nameof(GetTestStrings2))]
    public void CanExec_SqlString(string str)
    {
        var sqlValue = new SqlString(str);
        Assert.Equal(sqlValue.Value, Helper.Exec<string>("SELECT @Data", sqlValue));
    }

    [Fact]
    public void CanExec_Record()
    {
        var item1 = new Item { Id = 1, Name = "John" };

        Assert.Equal(item1.Name, Helper.Exec<string?>("SELECT JSON_VALUE(@Data, '$.Name')", item1));

        var rec1 = Helper.Exec<JsonObject>("SELECT @Data", item1);
        Assert.NotNull(rec1);
        Assert.Equal(1, (int?)rec1["Id"]);
        Assert.Equal("John", (string?)rec1["Name"]);
    }

    [Fact]
    public void CanExec_Crud()
    {
        var item1 = new { Name = "HelperName1" };

        var id = Helper.Exec<int?>("INSERT INTO TestItems (Name) OUTPUT INSERTED.Id VALUES (@Name)", new { item1.Name });
        Assert.NotNull(id);

        var item2 = Helper.Read<Item>("SELECT * FROM TestItems WHERE Id = @Data", id).FirstOrDefault();
        Assert.NotNull(item2);
        Assert.Equal(id, item2.Id);
        Assert.Equal(item1.Name, item2.Name);
        Assert.Null(item2?.Date);

        Assert.NotNull(item2);
        item2.Name = "HelperName2";
        item2.Date = DateTime.Today;

        Helper.Exec("UPDATE TestItems SET Name = @Name, Date = @Date WHERE Id = @Id", new { item2.Id, item2.Name, item2.Date });

        var item3 = Helper.Read<Item>("SELECT * FROM TestItems WHERE Id = @Data", id).FirstOrDefault();
        Assert.NotNull(item3);
        Assert.Equal(item2.Id, item3.Id);
        Assert.Equal(item2.Name, item3.Name);
        Assert.Equal(DateTime.Today, item3.Date);

        var count = Helper.Exec("DELETE TestItems WHERE Id = @Data", id);
        Assert.Equal(1, count);

        var item4 = Helper.Read<Item>("SELECT * FROM TestItems WHERE Id = @Data", id).FirstOrDefault();
        Assert.Null(item4);
    }
}