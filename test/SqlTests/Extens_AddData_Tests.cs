using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json;
using System.Xml;

namespace UkrGuru.Sql.Tests;

public partial class Extens_AddData_Tests
{
    [Fact]
    public void AddData_WithNullValue_AddsParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;

        // Act
        parameters.AddData(null);

        // Assert
        Assert.Empty(parameters);
    }

    [Fact]
    public void AddData_WithDBNullValue_AddsParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        var dbNullValue = DBNull.Value;

        // Act
        parameters.AddData(dbNullValue);

        // Assert
        Assert.Single(parameters);
        Assert.Equal(DBNull.Value, parameters["@Data"].Value);
    }

    [Fact]
    public void AddData_WithAllDotNetTypes_AddsParameters()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        var dotNetTypes = new (string Name, object Value)[]
        {
            ("@Boolean", true),
            ("@Byte", (byte)123),
            ("@ByteArray", new byte[] { 0x01, 0x02 }),
            ("@Char", 'A'),
            ("@DateTime", DateTime.Now),
            ("@Decimal", 123.45m),
            ("@Double", 123.45),
            ("@Guid", Guid.NewGuid()),
            ("@Int16", (short)123),
            ("@Int32", 123),
            ("@Int64", 1234567890123456789L),
            ("@Single", 123.45f),
            ("@String", "Test"),
            ("@TimeSpan", TimeSpan.FromHours(1)),
            ("@UInt16", (ushort)123),
            ("@UInt32", (uint)123),
            ("@UInt64", (ulong)1234567890123456789L),
            ("@Xml", "<root></root>")
        };

        // Act
        foreach (var (name, value) in dotNetTypes)
        {
            parameters.AddData(new SqlParameter(name, value));
        }

        // Assert
        Assert.Equal(dotNetTypes.Length, parameters.Count);
        foreach (var (name, value) in dotNetTypes)
        {
            Assert.Equal(value, parameters[name].Value);
        }
    }

    [Fact]
    public void AddData_WithAllSqlTypes_AddsParameters()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        var sqlTypes = new (string Name, object Value, SqlDbType Type)[]
        {
            ("@BigInt", 1234567890123456789L, SqlDbType.BigInt),
            ("@Binary", new byte[] { 0x01, 0x02 }, SqlDbType.Binary),
            ("@Bit", true, SqlDbType.Bit),
            ("@Char", 'A', SqlDbType.Char),
            ("@Date", DateTime.Now.Date, SqlDbType.Date),
            ("@DateTime", DateTime.Now, SqlDbType.DateTime),
            ("@Decimal", 123.45m, SqlDbType.Decimal),
            ("@Float", 123.45, SqlDbType.Float),
            ("@Image", new byte[] { 0x01, 0x02 }, SqlDbType.Image),
            ("@Int", 123, SqlDbType.Int),
            ("@Money", 123.45m, SqlDbType.Money),
            ("@NChar", 'A', SqlDbType.NChar),
            ("@NText", "Test", SqlDbType.NText),
            ("@NVarChar", "Test", SqlDbType.NVarChar),
            ("@Real", 123.45f, SqlDbType.Real),
            ("@SmallDateTime", DateTime.Now, SqlDbType.SmallDateTime),
            ("@SmallInt", 123, SqlDbType.SmallInt),
            ("@SmallMoney", 123.45m, SqlDbType.SmallMoney),
            ("@Text", "Test", SqlDbType.Text),
            ("@Time", DateTime.Now.TimeOfDay, SqlDbType.Time),
            ("@Timestamp", new byte[] { 0x01, 0x02 }, SqlDbType.Timestamp),
            ("@TinyInt", 123, SqlDbType.TinyInt),
            ("@UniqueIdentifier", Guid.NewGuid(), SqlDbType.UniqueIdentifier),
            ("@VarBinary", new byte[] { 0x01, 0x02 }, SqlDbType.VarBinary),
            ("@VarChar", "Test", SqlDbType.VarChar),
            ("@Xml", "<root></root>", SqlDbType.Xml)
        };

        // Act
        foreach (var (name, value, type) in sqlTypes)
        {
            parameters.AddData(new SqlParameter(name, type) { Value = value });
        }

        // Assert
        Assert.Equal(sqlTypes.Length, parameters.Count);
        foreach (var (name, value, _) in sqlTypes)
        {
            Assert.Equal(value, parameters[name].Value);
        }
    }

    [Fact]
    public void AddData_WithSqlParameter_AddsParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        var sqlParameter = new SqlParameter("@Test", SqlDbType.Int) { Value = 1 };

        // Act
        parameters.AddData(sqlParameter);

        // Assert
        Assert.Single(parameters);
        Assert.Equal(sqlParameter, parameters["@Test"]);
    }

    [Fact]
    public void AddData_WithSqlParameters_AddsParameters()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        var sqlParameters = new[]
        {
            new SqlParameter("@Id", 1),
            new SqlParameter("@Name", "Test")
        };

        // Act
        parameters.AddData(sqlParameters);

        // Assert
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters["@Id"].Value);
        Assert.Equal("Test", parameters["@Name"].Value);
    }

    [Fact]
    public void AddData_WithAnonymousType_AddsParameters()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        var anonymousObject = new { Id = 1, Name = "Test" };

        // Act
        parameters.AddData(anonymousObject);

        // Assert
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters["@Id"].Value);
        Assert.Equal("Test", parameters["@Name"].Value);
    }

    [Fact]
    public void AddData_WithNamedType_AddsParameters()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        var data = new NamedType { Id = 1, Name = "Test" };

        // Act
        parameters.AddData(data);

        // Assert
        Assert.Equal(2, parameters.Count);
        Assert.Equal(1, parameters["@Id"].Value);
        Assert.Equal("Test", parameters["@Name"].Value);
    }

    [Fact]
    public void AddData_WithObject_AddsSerializedParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        object data = new { Id = 1, Name = "Test" };

        // Act
        parameters.AddData(data.ToJson());

        // Assert
        Assert.Single(parameters);
        Assert.Equal(JsonSerializer.Serialize(data), parameters["@Data"].Value);
    }

    [Fact]
    public void AddData_WithEnum_AddsAsSingleParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;

        // Act
        parameters.AddData(TestEnum.Value2);

        // Assert
        Assert.Single(parameters);
        Assert.Equal(TestEnum.Value2, parameters["@Data"].Value);
    }

    [Fact]
    public void AddData_WithStream_AddsAsSingleParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        using var stream = new MemoryStream([1, 2, 3]);

        // Act
        parameters.AddData(stream);

        // Assert
        Assert.Single(parameters);
        Assert.Equal(stream, parameters["@Data"].Value);
    }

    [Fact]
    public void AddData_WithTextReader_AddsAsSingleParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        using var reader = new StringReader("hello");

        // Act
        parameters.AddData(reader);

        // Assert
        Assert.Single(parameters);
        Assert.Equal(reader, parameters["@Data"].Value);
    }

    [Fact]
    public void AddData_WithXmlReader_AddsAsSingleParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;
        using var stringReader = new StringReader("<root />");
        using var xmlReader = XmlReader.Create(stringReader);

        // Act
        parameters.AddData(xmlReader);

        // Assert
        Assert.Single(parameters);
        Assert.Equal(xmlReader, parameters["@Data"].Value);
    }

    [Fact]
    public void AddData_WithComplexObject_WhereSqlValueNotNull_AddsAsSingleParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;

        // SqlParameter(SqlValue) is NOT null for string
        var data = (object)"TestString";

        // Act
        parameters.AddData(data);

        // Assert
        Assert.Single(parameters);
        Assert.Equal("TestString", parameters["@Data"].Value);
    }

    [Fact]
    public void AddData_WithSqlParameterWithNullValue_AddsParameter()
    {
        // Arrange
        var parameters = new SqlCommand().Parameters;

        var sqlParam = new SqlParameter("@Test", DBNull.Value);

        // Act
        parameters.AddData(sqlParam);

        // Assert
        Assert.Single(parameters);
        Assert.Equal(DBNull.Value, parameters["@Test"].Value);
    }

    public enum TestEnum
    {
        Value1,
        Value2,
        Value3
    }

    private class NamedType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}