using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Text.Json;
using System.Text;

namespace DeserializeJEBenchmark
{
    [MemoryDiagnoser]
    [IterationCount(50)]       // More iterations for stability
    [WarmupCount(10)]          // Extended warm-up for JIT optimization
    [MinIterationTime(500)]    // Minimum 500ms per iteration for better accuracy
    public class DeserializeJEBenchmark
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DeserializeJEBenchmark>();
        }

        private static readonly object?[] TestInput = new object?[]
        {
            null,
            false,
            true,
            (byte)0x0a,
            Encoding.UTF8.GetBytes("\n\r"),
            'A',
            new char[] { 'A', 'V' },
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
            "ASD",
            new string[] { "A", "V" }
        };

        private readonly JsonElement[] _inputElements;
        private readonly JsonSerializerOptions _options;

        public DeserializeJEBenchmark()
        {
            _inputElements = new JsonElement[TestInput.Length];
            _options = new JsonSerializerOptions();
            for (int i = 0; i < TestInput.Length; i++)
            {
                string json = TestInput[i] switch
                {
                    null => "null",
                    byte[] bytes => JsonSerializer.Serialize(Convert.ToBase64String(bytes)),
                    _ => JsonSerializer.Serialize(TestInput[i])
                };
                _inputElements[i] = JsonDocument.Parse(json).RootElement;
            }
        }

        [Benchmark]
        public object?[] IfElse()
        {
            object?[] values = new object?[_inputElements.Length * 2]; // Double size for type + string
            int index = 0;

            values[index++] = DeserializeJE_IfElse<bool?>(_inputElements[0], null, _options); // null
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[0], null, _options);

            values[index++] = DeserializeJE_IfElse<bool>(_inputElements[1], false, _options); // false
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[1], null, _options);

            values[index++] = DeserializeJE_IfElse<bool>(_inputElements[2], false, _options); // true
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[2], null, _options);

            values[index++] = DeserializeJE_IfElse<byte>(_inputElements[3], 0, _options); // byte
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[3], null, _options);

            values[index++] = DeserializeJE_IfElse<byte[]>(_inputElements[4], null, _options); // byte[]
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[4], null, _options);

            values[index++] = DeserializeJE_IfElse<char>(_inputElements[5], '\0', _options); // char
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[5], null, _options);

            values[index++] = DeserializeJE_IfElse<char[]>(_inputElements[6], null, _options); // char[]
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[6], null, _options);

            values[index++] = DeserializeJE_IfElse<DateOnly>(_inputElements[7], default, _options); // DateOnly
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[7], null, _options);

            values[index++] = DeserializeJE_IfElse<TimeOnly>(_inputElements[8], default, _options); // TimeOnly
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[8], null, _options);

            values[index++] = DeserializeJE_IfElse<DateTime>(_inputElements[9], default, _options); // DateTime
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[9], null, _options);

            values[index++] = DeserializeJE_IfElse<decimal>(_inputElements[10], 0m, _options); // decimal
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[10], null, _options);

            values[index++] = DeserializeJE_IfElse<double>(_inputElements[11], 0d, _options); // double
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[11], null, _options);

            values[index++] = DeserializeJE_IfElse<TestEnum>(_inputElements[12], default, _options); // enum
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[12], null, _options);

            values[index++] = DeserializeJE_IfElse<Guid>(_inputElements[13], default, _options); // Guid
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[13], null, _options);

            values[index++] = DeserializeJE_IfElse<short>(_inputElements[14], 0, _options); // short
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[14], null, _options);

            values[index++] = DeserializeJE_IfElse<int>(_inputElements[15], 0, _options); // int
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[15], null, _options);

            values[index++] = DeserializeJE_IfElse<long>(_inputElements[16], 0L, _options); // long
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[16], null, _options);

            values[index++] = DeserializeJE_IfElse<string>(_inputElements[17], null, _options); // string
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[17], null, _options); // string as string

            values[index++] = DeserializeJE_IfElse<string[]>(_inputElements[18], null, _options); // string[]
            values[index++] = DeserializeJE_IfElse<string>(_inputElements[18], null, _options);

            return values;
        }

        [Benchmark]
        public object?[] Switch()
        {
            object?[] values = new object?[_inputElements.Length * 2]; // Double size for type + string
            int index = 0;

            values[index++] = DeserializeJE_Switch<bool?>(_inputElements[0], null, _options); // null
            values[index++] = DeserializeJE_Switch<string>(_inputElements[0], null, _options);

            values[index++] = DeserializeJE_Switch<bool>(_inputElements[1], false, _options); // false
            values[index++] = DeserializeJE_Switch<string>(_inputElements[1], null, _options);

            values[index++] = DeserializeJE_Switch<bool>(_inputElements[2], false, _options); // true
            values[index++] = DeserializeJE_Switch<string>(_inputElements[2], null, _options);

            values[index++] = DeserializeJE_Switch<byte>(_inputElements[3], 0, _options); // byte
            values[index++] = DeserializeJE_Switch<string>(_inputElements[3], null, _options);

            values[index++] = DeserializeJE_Switch<byte[]>(_inputElements[4], null, _options); // byte[]
            values[index++] = DeserializeJE_Switch<string>(_inputElements[4], null, _options);

            values[index++] = DeserializeJE_Switch<char>(_inputElements[5], '\0', _options); // char
            values[index++] = DeserializeJE_Switch<string>(_inputElements[5], null, _options);

            values[index++] = DeserializeJE_Switch<char[]>(_inputElements[6], null, _options); // char[]
            values[index++] = DeserializeJE_Switch<string>(_inputElements[6], null, _options);

            values[index++] = DeserializeJE_Switch<DateOnly>(_inputElements[7], default, _options); // DateOnly
            values[index++] = DeserializeJE_Switch<string>(_inputElements[7], null, _options);

            values[index++] = DeserializeJE_Switch<TimeOnly>(_inputElements[8], default, _options); // TimeOnly
            values[index++] = DeserializeJE_Switch<string>(_inputElements[8], null, _options);

            values[index++] = DeserializeJE_Switch<DateTime>(_inputElements[9], default, _options); // DateTime
            values[index++] = DeserializeJE_Switch<string>(_inputElements[9], null, _options);

            values[index++] = DeserializeJE_Switch<decimal>(_inputElements[10], 0m, _options); // decimal
            values[index++] = DeserializeJE_Switch<string>(_inputElements[10], null, _options);

            values[index++] = DeserializeJE_Switch<double>(_inputElements[11], 0d, _options); // double
            values[index++] = DeserializeJE_Switch<string>(_inputElements[11], null, _options);

            values[index++] = DeserializeJE_Switch<TestEnum>(_inputElements[12], default, _options); // enum
            values[index++] = DeserializeJE_Switch<string>(_inputElements[12], null, _options);

            values[index++] = DeserializeJE_Switch<Guid>(_inputElements[13], default, _options); // Guid
            values[index++] = DeserializeJE_Switch<string>(_inputElements[13], null, _options);

            values[index++] = DeserializeJE_Switch<short>(_inputElements[14], 0, _options); // short
            values[index++] = DeserializeJE_Switch<string>(_inputElements[14], null, _options);

            values[index++] = DeserializeJE_Switch<int>(_inputElements[15], 0, _options); // int
            values[index++] = DeserializeJE_Switch<string>(_inputElements[15], null, _options);

            values[index++] = DeserializeJE_Switch<long>(_inputElements[16], 0L, _options); // long
            values[index++] = DeserializeJE_Switch<string>(_inputElements[16], null, _options);

            values[index++] = DeserializeJE_Switch<string>(_inputElements[17], null, _options); // string
            values[index++] = DeserializeJE_Switch<string>(_inputElements[17], null, _options); // string as string

            values[index++] = DeserializeJE_Switch<string[]>(_inputElements[18], null, _options); // string[]
            values[index++] = DeserializeJE_Switch<string>(_inputElements[18], null, _options);

            return values;
        }

        // If-Else Version
        private static T? DeserializeJE_IfElse<T>(JsonElement je, T? defaultValue = default, JsonSerializerOptions? options = null)
        {
            if (je.ValueKind == JsonValueKind.Null) return defaultValue;

            Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
            if (targetType == typeof(string))
            {
                return (T)(je.ValueKind == JsonValueKind.String
                    ? je.GetString()!
                    : je.GetRawText().Trim('"') as object);
            }

            return je.Deserialize<T>(options);
        }

        // Switch Version
        private static T? DeserializeJE_Switch<T>(JsonElement je, T? defaultValue = default, JsonSerializerOptions? options = null) =>
            je.ValueKind switch
            {
                JsonValueKind.Null => defaultValue,
                _ => (Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T)) switch
                {
                    Type t when t == typeof(string) => (T)(je.ValueKind switch
                    {
                        JsonValueKind.String => je.GetString()!,
                        _ => je.GetRawText().Trim('"') as object
                    }),
                    _ => je.Deserialize<T>(options)
                }
            };
    }

    public enum TestEnum { Zero, One }
}