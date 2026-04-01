using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using UkrGuru.Sql;

//[ShortRunJob]
//[MemoryDiagnoser]
public class SqlBenchmark
{
    private const string ConnectionString =
        "Server=(local);Database=SampleStoreLarge;Trusted_Connection=True;TrustServerCertificate=True;";

    private const string CommandText =
        "SELECT CustomerId, FullName, Email, CreatedAt FROM Customers";

    [Benchmark]
    public async Task<int> UkrGuru_LoadList()
    {
        await using var connection = await DbHelper.CreateConnectionAsync(ConnectionString);

        var list = await connection.ReadAsync<Customer>(CommandText);

        return list.Count();
    }

    [Benchmark]
    public async Task<int> UkrGuru_StreamRows()
    {
        int count = 0;

        await using var command = await DbHelper.CreateCommandAsync(CommandText, connectionString: ConnectionString);

        await foreach (var item in command.ReadAsync<Customer>())
            count++;

        return count;
    }

    [Benchmark]
    public async Task<int> ADONET_LoadList()
    {
        var list = new List<Customer>();

        using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync();

        using var cmd = new SqlCommand(CommandText, conn);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            list.Add(new Customer
            {
                CustomerId = reader.GetInt32(0),
                FullName = reader.GetString(1),
                Email = reader.GetString(2),
                CreatedAt = reader.GetDateTime(3)
            });
        }

        return list.Count;
    }

    [Benchmark]
    public async Task<int> ADONET_StreamRows()
    {
        int count = 0;

        await foreach (var row in StreamCustomersAsync())
            count++;

        return count;

        async IAsyncEnumerable<Customer> StreamCustomersAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var conn = new SqlConnection(ConnectionString);
            await conn.OpenAsync(cancellationToken);

            using var cmd = new SqlCommand(CommandText, conn);

            using var reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess, cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                yield return new Customer
                {
                    CustomerId = reader.GetInt32(0),
                    FullName = reader.GetString(1),
                    Email = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3)
                };
            }
        }
    }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        //var bench = new SqlBenchmark();
        //Console.WriteLine(await bench.UkrGuru_LoadList());
        //Console.WriteLine(await bench.UkrGuru_StreamRows());
        //Console.WriteLine(await bench.ADONET_LoadList());
        //Console.WriteLine(await bench.ADONET_StreamRows());

        BenchmarkRunner.Run<SqlBenchmark>();
    }
}


//BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.8039/25H2/2025Update/HudsonValley2)
//12th Gen Intel Core i7-12700K 3.60GHz, 1 CPU, 20 logical and 12 physical cores
//.NET SDK 10.0.201
//  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3 [AttachedDebugger]
//DefaultJob: .NET 10.0.5(10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3


//| Method             | Mean     | Error     | StdDev    |
//|------------------- |---------:| ----------:| ----------:|
//| ADONET_LoadList | 4.309 ms | 0.0721 ms | 0.0675 ms |
//| UkrGuru_LoadList | 4.713 ms | 0.0908 ms | 0.1116 ms |
//| ADONET_StreamRows | 2.953 ms | 0.0472 ms | 0.0441 ms |
//| UkrGuru_StreamRows | 3.141 ms | 0.0415 ms | 0.0407 ms |
