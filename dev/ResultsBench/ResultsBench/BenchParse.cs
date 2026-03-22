using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

// Your two Results versions:
using UkrGuruOld = UkrGuru.Sql.Results_UkrGuru;
using CopilotNew = UkrGuru.Sql.Results_Copilot;

[ShortRunJob]
[MemoryDiagnoser]
[RankColumn]
public class BenchParse
{
    private const string boolText = "true";
    private const string intText = "123456";
    private const string doubleText = "123.456";
    private const string decimalText = "12345.6789";
    private readonly Guid guidValue = Guid.NewGuid();
    private readonly string guidText;
    private readonly string dateOnlyText = "2024-03-21";
    private readonly string timeOnlyText = "12:30:45.1234567";
    private readonly string timeSpanText = "12:30:45";

    private readonly byte[] bytes = new byte[] { 1, 2, 3, 4 };
    private readonly string base64;

    public BenchParse()
    {
        guidText = guidValue.ToString();
        base64 = Convert.ToBase64String(bytes);
    }

    // ----------------------------
    // BOOL
    // ----------------------------
    [Benchmark] public bool Old_Bool() => UkrGuruOld.Parse<bool>(boolText);
    [Benchmark] public bool New_Bool() => CopilotNew.Parse<bool>(boolText);

    // ----------------------------
    // INT
    // ----------------------------
    [Benchmark] public int Old_Int() => UkrGuruOld.Parse<int>(intText);
    [Benchmark] public int New_Int() => CopilotNew.Parse<int>(intText);

    // ----------------------------
    // DOUBLE
    // ----------------------------
    [Benchmark] public double Old_Double() => UkrGuruOld.Parse<double>(doubleText);
    [Benchmark] public double New_Double() => CopilotNew.Parse<double>(doubleText);

    // ----------------------------
    // DECIMAL
    // ----------------------------
    [Benchmark] public decimal Old_Decimal() => UkrGuruOld.Parse<decimal>(decimalText);
    [Benchmark] public decimal New_Decimal() => CopilotNew.Parse<decimal>(decimalText);

    // ----------------------------
    // GUID
    // ----------------------------
    [Benchmark] public Guid Old_Guid() => UkrGuruOld.Parse<Guid>(guidText);
    [Benchmark] public Guid New_Guid() => CopilotNew.Parse<Guid>(guidText);

    // ----------------------------
    // DateOnly
    // ----------------------------
    [Benchmark] public DateOnly Old_DateOnly() => UkrGuruOld.Parse<DateOnly>(dateOnlyText);
    [Benchmark] public DateOnly New_DateOnly() => CopilotNew.Parse<DateOnly>(dateOnlyText);

    // ----------------------------
    // TimeOnly
    // ----------------------------
    [Benchmark] public TimeOnly Old_TimeOnly() => UkrGuruOld.Parse<TimeOnly>(timeOnlyText);
    [Benchmark] public TimeOnly New_TimeOnly() => CopilotNew.Parse<TimeOnly>(timeOnlyText);

    // ----------------------------
    // TimeSpan
    // ----------------------------
    [Benchmark] public TimeSpan Old_TimeSpan() => UkrGuruOld.Parse<TimeSpan>(timeSpanText);
    [Benchmark] public TimeSpan New_TimeSpan() => CopilotNew.Parse<TimeSpan>(timeSpanText);

    // ----------------------------
    // byte[]
    // ----------------------------
    [Benchmark] public byte[] Old_ByteArray() => UkrGuruOld.Parse<byte[]>(base64);
    [Benchmark] public byte[] New_ByteArray() => CopilotNew.Parse<byte[]>(base64);

    // ----------------------------
    // char[]
    // ----------------------------
    private readonly char[] chars = "Hello".ToCharArray();

    [Benchmark] public char[] Old_CharArray() => UkrGuruOld.Parse<char[]>(chars);
    [Benchmark] public char[] New_CharArray() => CopilotNew.Parse<char[]>(chars);

    // ----------------------------
    // string serialize
    // ----------------------------
    [Benchmark] public string Old_Serialize() => UkrGuruOld.Parse<string>(12345);
    [Benchmark] public string New_Serialize() => CopilotNew.Parse<string>(12345);

    // ----------------------------
    // Enum
    // ----------------------------
    public enum TestEnum { One, Two }

    [Benchmark] public TestEnum Old_Enum() => UkrGuruOld.Parse<TestEnum>("Two");
    [Benchmark] public TestEnum New_Enum() => CopilotNew.Parse<TestEnum>("Two");
}