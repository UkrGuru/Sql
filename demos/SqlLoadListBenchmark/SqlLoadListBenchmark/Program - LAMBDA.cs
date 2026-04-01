using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Data.SqlClient;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;
using System;
using System.Linq.Expressions;
using System.Reflection;

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

// ------------------ ORIGINAL ------------------

public static class OriginalMapper
{
    public static object Map(PropertyInfo[] props, object[] values)
    {
        var obj = new Customer();
        for (int i = 0; i < props.Length; i++)
            props[i].SetValue(obj, values[i]);

        return obj;
    }
}

// ------------------ LAMBDA MAPPER ------------------

public class LambdaMapper
{
    public Action<object, object?>[] Setters;
    public Func<object> Ctor;
    public PropertyInfo[] Props;

    public LambdaMapper(Type t)
    {
        Props = t.GetProperties();
        Setters = new Action<object, object?>[Props.Length];

        for (int i = 0; i < Props.Length; i++)
        {
            var target = Expression.Parameter(typeof(object));
            var value = Expression.Parameter(typeof(object));

            var body = Expression.Assign(
                Expression.Property(Expression.Convert(target, t), Props[i]),
                Expression.Convert(value, Props[i].PropertyType));

            Setters[i] = Expression.Lambda<Action<object, object?>>(body, target, value).Compile();
        }

        Ctor = Expression.Lambda<Func<object>>(Expression.New(t)).Compile();
    }

    public object Map(object[] values)
    {
        var obj = Ctor();
        for (int i = 0; i < Props.Length; i++)
            Setters[i](obj, values[i]);
        return obj;
    }
}

// ------------------ BENCHMARK ------------------

[MemoryDiagnoser]
public class MappingBenchmark
{
    private readonly object[] _values =
    {
        123,
        "John Doe",
        "john@example.com",
        DateTime.UtcNow
    };

    private readonly PropertyInfo[] _props = typeof(Customer).GetProperties();

    private readonly LambdaMapper _lambda = new(typeof(Customer));

    [Benchmark]
    public object Original_Reflection()
        => OriginalMapper.Map(_props, _values);

    [Benchmark]
    public object Lambda_Compiled()
        => _lambda.Map(_values);
}

public class Program
{
    public static void Main(string[] args)
        => BenchmarkRunner.Run<MappingBenchmark>();
}

//| Method | Mean | Error | StdDev | Gen0 | Allocated |
//| -------------------- | ---------:| ---------:| ---------:| -------:| ----------:|
//| Original_Reflection | 30.96 ns | 0.212 ns | 0.198 ns | 0.0036 | 48 B |
//| Lambda_Compiled | 12.57 ns | 0.071 ns | 0.059 ns | 0.0037 | 48 B |