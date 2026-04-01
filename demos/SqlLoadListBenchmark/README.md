# **SqlLoadListBenchmark – Simple ADO.NET vs UkrGuru.Sql Demo**

This demo shows how **ADO.NET** and **UkrGuru.Sql** perform the same work using different coding styles, along with benchmark results.

***

## 🖼 **1. LoadList Demo (ADO.NET vs UkrGuru)**

### **ADO.NET\_LoadList**

```csharp
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
            FullName   = reader.GetString(1),
            Email      = reader.GetString(2),
            CreatedAt  = reader.GetDateTime(3)
        });
    }

    return list.Count;
}
```

### **UkrGuru\_LoadList**

```csharp
public async Task<int> UkrGuru_LoadList()
{
    await using var connection = await DbHelper.CreateConnectionAsync(ConnectionString);
    var list = await connection.ReadAsync<Customer>(CommandText);
    return list.Count();
}
```

***

## 🖼 **2. StreamRows Demo (ADO.NET vs UkrGuru)**

### **ADO.NET\_StreamRows**

```csharp
public async Task<int> ADONET_StreamRows()
{
    int count = 0;
    await foreach (var row in StreamCustomersAsync())
        count++;

    return count;

    async IAsyncEnumerable<Customer> StreamCustomersAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var conn = new SqlConnection(ConnectionString);
        await conn.OpenAsync(cancellationToken);
        using var cmd = new SqlCommand(CommandText, conn);
        using var reader = await cmd.ExecuteReaderAsync(
            System.Data.CommandBehavior.SequentialAccess, cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            yield return new Customer
            {
                CustomerId = reader.GetInt32(0),
                FullName   = reader.GetString(1),
                Email      = reader.GetString(2),
                CreatedAt  = reader.GetDateTime(3)
            };
        }
    }
}
```

### **UkrGuru\_StreamRows**

```csharp
public async Task<int> UkrGuru_StreamRows()
{
    int count = 0;
    await using var command =
        await DbHelper.CreateCommandAsync(CommandText, connectionString: ConnectionString);

    await foreach (var item in command.ReadAsync<Customer>())
        count++;

    return count;
}
```

***

## 📊 **Benchmark Results**

| Method                  |     Mean |     Error |    StdDev |
| ----------------------- | -------: | --------: | --------: |
| **ADONET\_StreamRows**  | 2.953 ms | 0.0472 ms | 0.0441 ms |
| **UkrGuru\_StreamRows** | 3.141 ms | 0.0415 ms | 0.0407 ms |
| **ADONET\_LoadList**    | 4.309 ms | 0.0721 ms | 0.0675 ms |
| **UkrGuru\_LoadList**   | 4.713 ms | 0.0908 ms | 0.1116 ms |

***

## 📝 Summary

*   **StreamRows**: Both are fast; ADO.NET is slightly faster in this raw sequential benchmark.
*   **LoadList**: UkrGuru.Sql is a bit slower in this scenario but provides **much cleaner code** with far less boilerplate.
*   **Code simplicity vs raw speed**:
    *   ADO.NET = faster, more verbose
    *   UkrGuru.Sql = cleaner, safer, more maintainable
