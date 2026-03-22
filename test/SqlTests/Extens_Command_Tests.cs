using Microsoft.Data.SqlClient;
using System.Data;
using static UkrGuru.Sql.Tests.GlobalTests;

namespace UkrGuru.Sql.Tests;

public partial class Extens_Command_Tests
{
    public Extens_Command_Tests() => DbHelper.ConnectionString = TestConnectionString;

    // -----------------------
    //  STORED PROCEDURE NAME DETECTION
    // -----------------------
    [Fact]
    public void CreateCommand_WithStoredProcedureName_SetsCommandTypeStoredProcedure()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var cmd = conn.CreateCommand("MyProc");

        Assert.Equal(CommandType.StoredProcedure, cmd.CommandType);
        Assert.Equal("MyProc", cmd.CommandText);
        Assert.Equal(conn, cmd.Connection);
    }

    [Theory]
    [InlineData("dbo.MyProc")]
    [InlineData("[dbo].[MyProc]")]
    [InlineData("[schema].[Proc123]")]
    public void CreateCommand_WithSchemaQualifiedName_SetsStoredProcedure(string procName)
    {
        using var conn = new SqlConnection(TestConnectionString);

        var cmd = conn.CreateCommand(procName);

        Assert.Equal(CommandType.StoredProcedure, cmd.CommandType);
        Assert.Equal(procName, cmd.CommandText);
    }

    // -----------------------
    //  SQL TEXT (NOT A PROC)
    // -----------------------
    [Fact]
    public void CreateCommand_WithSqlText_LeavesCommandTypeAsText()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var cmd = conn.CreateCommand("SELECT 1");

        Assert.Equal(CommandType.Text, cmd.CommandType);
        Assert.Equal("SELECT 1", cmd.CommandText);
    }

    // -----------------------
    //  PARAMETER MAPPING
    // -----------------------
    [Fact]
    public void CreateCommand_WithDataObject_AddsParameters()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var data = new { Id = 10, Name = "Alex" };

        var cmd = conn.CreateCommand("MyProc", data);

        Assert.Equal(2, cmd.Parameters.Count);
        Assert.Equal(10, cmd.Parameters["@Id"].Value);
        Assert.Equal("Alex", cmd.Parameters["@Name"].Value);
    }

    [Fact]
    public void CreateCommand_WithScalar_AddsDataParameter()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var cmd = conn.CreateCommand("MyProc", 123);

        Assert.Single(cmd.Parameters);
        Assert.Equal("@Data", cmd.Parameters[0].ParameterName);
        Assert.Equal(123, cmd.Parameters[0].Value);
    }

    [Fact]
    public void CreateCommand_WithSqlParameter_AddsParameter()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var p = new SqlParameter("@Id", 5);
        var cmd = conn.CreateCommand("MyProc", p);

        Assert.Single(cmd.Parameters);
        Assert.Equal("@Id", cmd.Parameters[0].ParameterName);
        Assert.Equal(5, cmd.Parameters[0].Value);
    }

    [Fact]
    public void CreateCommand_WithSqlParameterArray_AddsParameters()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var arr = new[]
        {
            new SqlParameter("@A", 1),
            new SqlParameter("@B", 2)
        };

        var cmd = conn.CreateCommand("MyProc", arr);

        Assert.Equal(2, cmd.Parameters.Count);
    }

    // -----------------------
    //  COMMAND TIMEOUT
    // -----------------------
    [Fact]
    public void CreateCommand_WithTimeout_SetsCommandTimeout()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var cmd = conn.CreateCommand("MyProc", timeout: 25);

        Assert.Equal(25, cmd.CommandTimeout);
    }

    // -----------------------
    //  CONNECTION IS PROPERLY SET
    // -----------------------
    [Fact]
    public void CreateCommand_AlwaysSetsConnection()
    {
        using var conn = new SqlConnection(TestConnectionString);

        var cmd = conn.CreateCommand("SELECT 1");

        Assert.Equal(conn, cmd.Connection);
    }
}
