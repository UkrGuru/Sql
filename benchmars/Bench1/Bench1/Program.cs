using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Data.SqlClient;
using UkrGuru.Sql;

//internal class Program
//{
//    private static void Main(string[] args)
//    {
//        Tests t = new();
//        for (int i = 0; i < 10_000; i++)
//        {
//            t.TestAdo();
//        }
//    }
//}

BenchmarkSwitcher.FromAssembly(typeof(Tests).Assembly).Run(args);

[ShortRunJob]
[MemoryDiagnoser]
public class Tests
{
    public string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=SqlJsonTest;Trusted_Connection=True";

    public Tests() => DbHelper.ConnectionString = ConnectionString;

    [Benchmark]
    public int TestAdo()
    {
        int result = -1;
        using (var cnn = new SqlConnection(ConnectionString))
        {
            cnn.Open();
            SqlCommand cmd = new SqlCommand("DECLARE @Sum int = @Num1 + @Num2;", cnn);
            cmd.Parameters.AddWithValue("@Num1", 1);
            cmd.Parameters.AddWithValue("@Num2", 2);

            result = cmd.ExecuteNonQuery();
            cnn.Close();
        }
        return result;
    }

    [Benchmark]
    public int TestUkr() 
        => DbHelper.Exec("DECLARE @Sum int = @Num1 + @Num2;", new { Num1 = 1, Num2 = 2 });
}