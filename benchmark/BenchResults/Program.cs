﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Text;
using UkrGuru.Sql;

//var bench = new Benchmarks(); bench.ResultsNew_Parse();

var summary = BenchmarkRunner.Run<Benchmarks>();

public enum TestEnum { Zero, One }

public class NamedType
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

//[ShortRunJob]
[MemoryDiagnoser]
[IterationCount(50)]       // More iterations for stability
[WarmupCount(10)]          // Extended warm-up for JIT optimization
[MinIterationTime(500)]    // Minimum 500ms per iteration for better accuracy
public class Benchmarks
{
    private static readonly object?[] _inputElements =
    [
            null,
            false,
            true,
            (byte)0x0a,
            Encoding.UTF8.GetBytes("AV & ASD\n\r"),
            'A',
            new char[] { 'A', 'V', '&', 'A', 'S', 'D' },
            new DateOnly(2000, 11, 25),
            new TimeOnly(23, 59, 59),
            new DateTime(2000, 11, 25, 23, 59, 59),
            123456.789m,
            123456.789d,
            TestEnum.One,
            Guid.NewGuid(),
            (short)0,
            42,
            0L,
            "AV & ASD",
            new string[] { "A", "V", "&", "A", "S", "D" },
            new NamedType { Id = 1, Name = "Test" }
    ];

    [Benchmark]
    public object?[] ParseOld_40_results()
    {
        object?[] values = new object?[_inputElements.Length * 2]; // Double size for type + string
        int index = 0;

        values[index++] = ResultsOld.Parse<bool?>(_inputElements[0], null); // null
        values[index++] = ResultsOld.Parse<string>(_inputElements[0], null);

        values[index++] = ResultsOld.Parse<bool>(_inputElements[1], false); // false
        values[index++] = ResultsOld.Parse<string>(_inputElements[1], null);

        values[index++] = ResultsOld.Parse<bool>(_inputElements[2], false); // true
        values[index++] = ResultsOld.Parse<string>(_inputElements[2], null);

        values[index++] = ResultsOld.Parse<byte>(_inputElements[3], 0); // byte
        values[index++] = ResultsOld.Parse<string>(_inputElements[3], null);

        values[index++] = ResultsOld.Parse<byte[]>(_inputElements[4], null); // byte[]
        values[index++] = ResultsOld.Parse<string>(_inputElements[4], null);

        values[index++] = ResultsOld.Parse<char>(_inputElements[5], '\0'); // char
        values[index++] = ResultsOld.Parse<string>(_inputElements[5], null);

        values[index++] = ResultsOld.Parse<char[]>(_inputElements[6], null); // char[]
        values[index++] = ResultsOld.Parse<string>(_inputElements[6], null);

        values[index++] = ResultsOld.Parse<DateOnly>(_inputElements[7], default); // DateOnly
        values[index++] = ResultsOld.Parse<string>(_inputElements[7], null);

        values[index++] = ResultsOld.Parse<TimeOnly>(_inputElements[8], default); // TimeOnly
        values[index++] = ResultsOld.Parse<string>(_inputElements[8], null);

        values[index++] = ResultsOld.Parse<DateTime>(_inputElements[9], default); // DateTime
        values[index++] = ResultsOld.Parse<string>(_inputElements[9], null);

        values[index++] = ResultsOld.Parse<decimal>(_inputElements[10], 0m); // decimal
        values[index++] = ResultsOld.Parse<string>(_inputElements[10], null);

        values[index++] = ResultsOld.Parse<double>(_inputElements[11], 0d); // double
        values[index++] = ResultsOld.Parse<string>(_inputElements[11], null);

        values[index++] = ResultsOld.Parse<TestEnum>(_inputElements[12], default); // enum
        values[index++] = ResultsOld.Parse<string>(_inputElements[12], null);

        values[index++] = ResultsOld.Parse<Guid>(_inputElements[13], default); // Guid
        values[index++] = ResultsOld.Parse<string>(_inputElements[13], null);

        values[index++] = ResultsOld.Parse<short>(_inputElements[14], 0); // short
        values[index++] = ResultsOld.Parse<string>(_inputElements[14], null);

        values[index++] = ResultsOld.Parse<int>(_inputElements[15], 0); // int
        values[index++] = ResultsOld.Parse<string>(_inputElements[15], null);

        values[index++] = ResultsOld.Parse<long>(_inputElements[16], 0L); // long
        values[index++] = ResultsOld.Parse<string>(_inputElements[16], null);

        values[index++] = ResultsOld.Parse<string>(_inputElements[17], null); // string
        values[index++] = ResultsOld.Parse<string>(_inputElements[17], null); // string as string

        values[index++] = ResultsOld.Parse<string[]>(_inputElements[18], null); // string[]
        values[index++] = ResultsOld.Parse<string>(_inputElements[18], null);

        values[index++] = ResultsOld.Parse<NamedType>(_inputElements[19], null);

        return values;
    }

    [Benchmark]
    public object?[] ParsePro_40_results()
    {
        object?[] values = new object?[_inputElements.Length * 2]; // Double size for type + string
        int index = 0;

        values[index++] = ResultsPro.Parse<bool?>(_inputElements[0], null); // null
        values[index++] = ResultsPro.Parse<string>(_inputElements[0], null);

        values[index++] = ResultsPro.Parse<bool>(_inputElements[1], false); // false
        values[index++] = ResultsPro.Parse<string>(_inputElements[1], null);

        values[index++] = ResultsPro.Parse<bool>(_inputElements[2], false); // true
        values[index++] = ResultsPro.Parse<string>(_inputElements[2], null);

        values[index++] = ResultsPro.Parse<byte>(_inputElements[3], 0); // byte
        values[index++] = ResultsPro.Parse<string>(_inputElements[3], null);

        values[index++] = ResultsPro.Parse<byte[]>(_inputElements[4], null); // byte[]
        values[index++] = ResultsPro.Parse<string>(_inputElements[4], null);

        values[index++] = ResultsPro.Parse<char>(_inputElements[5], '\0'); // char
        values[index++] = ResultsPro.Parse<string>(_inputElements[5], null);

        values[index++] = ResultsPro.Parse<char[]>(_inputElements[6], null); // char[]
        values[index++] = ResultsPro.Parse<string>(_inputElements[6], null);

        values[index++] = ResultsPro.Parse<DateOnly>(_inputElements[7], default); // DateOnly
        values[index++] = ResultsPro.Parse<string>(_inputElements[7], null);

        values[index++] = ResultsPro.Parse<TimeOnly>(_inputElements[8], default); // TimeOnly
        values[index++] = ResultsPro.Parse<string>(_inputElements[8], null);

        values[index++] = ResultsPro.Parse<DateTime>(_inputElements[9], default); // DateTime
        values[index++] = ResultsPro.Parse<string>(_inputElements[9], null);

        values[index++] = ResultsPro.Parse<decimal>(_inputElements[10], 0m); // decimal
        values[index++] = ResultsPro.Parse<string>(_inputElements[10], null);

        values[index++] = ResultsPro.Parse<double>(_inputElements[11], 0d); // double
        values[index++] = ResultsPro.Parse<string>(_inputElements[11], null);

        values[index++] = ResultsPro.Parse<TestEnum>(_inputElements[12], default); // enum
        values[index++] = ResultsPro.Parse<string>(_inputElements[12], null);

        values[index++] = ResultsPro.Parse<Guid>(_inputElements[13], default); // Guid
        values[index++] = ResultsPro.Parse<string>(_inputElements[13], null);

        values[index++] = ResultsPro.Parse<short>(_inputElements[14], 0); // short
        values[index++] = ResultsPro.Parse<string>(_inputElements[14], null);

        values[index++] = ResultsPro.Parse<int>(_inputElements[15], 0); // int
        values[index++] = ResultsPro.Parse<string>(_inputElements[15], null);

        values[index++] = ResultsPro.Parse<long>(_inputElements[16], 0L); // long
        values[index++] = ResultsPro.Parse<string>(_inputElements[16], null);

        values[index++] = ResultsPro.Parse<string>(_inputElements[17], null); // string
        values[index++] = ResultsPro.Parse<string>(_inputElements[17], null); // string as string

        values[index++] = ResultsPro.Parse<string[]>(_inputElements[18], null); // string[]
        values[index++] = ResultsPro.Parse<string>(_inputElements[18], null);
        
        values[index++] = ResultsPro.Parse<NamedType>(_inputElements[19], null);

        return values;
    }
}