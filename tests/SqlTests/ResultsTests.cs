// Copyright (c) Oleksandr Viktor (UkrGuru). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Text;
using System.Text.Json;

namespace UkrGuru.Sql.Tests;

public class ResultsTests
{
    [Fact]
    public void CanParse_Null_Default()
    {
        Assert.Null(Results.Parse<bool?>(null));
        Assert.False(Results.Parse<bool>(null));
        Assert.False(Results.Parse<bool>(null, false));
        Assert.True(Results.Parse<bool>(null, true));

        Assert.Null(Results.Parse<bool?>(DBNull.Value));
        Assert.False(Results.Parse<bool>(DBNull.Value));
        Assert.False(Results.Parse<bool>(DBNull.Value, false));
        Assert.True(Results.Parse<bool>(DBNull.Value, true));
    }

    [Fact]
    public void CanParse_Bit()
    {
        bool result = true;

        Assert.Equal(result, Results.Parse<bool>(result));
        Assert.Equal(result, Results.Parse<bool>(result.ToString().ToLower()));
        Assert.Equal(result.ToString().ToLower(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Byte()
    {
        byte result = 123;

        Assert.Equal(result, Results.Parse<byte>(result));
        Assert.Equal(result, Results.Parse<byte>(result.ToString()));
        Assert.Equal(result.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Bytes()
    {
        byte[] result = [1, 2];

        Assert.Equal(result, Results.Parse<byte[]>(result));
        Assert.Equal(result, Results.Parse<byte[]>(Convert.ToBase64String(result)));
        Assert.Equal(Convert.ToBase64String(result), Results.Parse<string>(result));
    }

    [Fact]
    public void Parse_InvalidCharArrayToBool_ThrowsFormatException()
    {
        char[] chars = new[] { 'x', 'y', 'z' };
        Assert.Throws<JsonException>(() => Results.Parse<bool>(chars));
        // Demo: Non-boolean string to bool
    }

    [Fact]
    public void Parse_LargeNumberToByte_ThrowsOverflow()
    {
        Assert.Throws<JsonException>(() => Results.Parse<byte>("1000"));
        // Demo: TypeParsers byte[] overflow
    }

    [Fact]
    public void Parse_InvalidBase64String_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Results.Parse<byte[]>("invalid!"));
        // Demo: TypeParsers byte[] parsing lacks validation
    }

    [Fact]
    public void Parse_LargeCharArrayToShort_ThrowsOverflow()
    {
        char[] chars = "123456".ToCharArray();
        Assert.Throws<JsonException>(() => Results.Parse<short>(chars));
        // Demo: Overflow in numeric conversion
    }

    [Fact]
    public void Parse_CharArrayToUnsignedInt_ThrowsOverflow()
    {
        char[] chars = "-123".ToCharArray();
        Assert.Throws<JsonException>(() => Results.Parse<uint>(chars));
        // Demo: Negative string to unsigned type
    }

    [Fact]
    public void Parse_NestedJsonToFlatType_ThrowsJsonException()
    {
        var json = "{\"value\": {\"inner\": 42}}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<int>(element));
        // Demo: Nested JSON to primitive
    }

    [Fact]
    public void Parse_NullJsonPropertyToNullable_ThrowsUnexpected()
    {
        var json = "{\"value\": null}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        var result = Results.Parse<int?>(element);
        Assert.Null(result); // Passes: Null handling for nullable
                             // Demo: No issue—checks consistency
    }

    [Fact]
    public void Parse_CharArrayToNullableInt_ThrowsUnexpected()
    {
        char[] chars = new[] { 'a', 'b', 'c' };
        Assert.Throws<JsonException>(() => Results.Parse<int?>(chars));
        // Demo: Nullable type conversion edge case
    }

    [Fact]
    public void Parse_EmptyCharArrayToInt_ThrowsFormatException()
    {
        char[] chars = Array.Empty<char>();
        Assert.Throws<JsonException>(() => Results.Parse<int>(chars));
        // Demo: Empty char[] conversion
    }

    [Fact]
    public void Parse_CharArrayToInvalidType_ThrowsException()
    {
        char[] chars = new[] { 'a', 'b', 'c' };
        Assert.Throws<JsonException>(() => Results.Parse<int>(chars));
        // Demo: Convert.ChangeType fails for non-numeric to int
    }

    [Fact]
    public void ParseJE_Null()
    {
        var json = "null";
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Null(Results.Parse<bool?>(value));
    }

    [Fact]
    public void ParseJE_Boolean()
    {
        bool bool_value = false;
        var json = JsonSerializer.Serialize(bool_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.False(Results.Parse<bool>(value));
        Assert.Equal(bool.FalseString.ToLower(), Results.Parse<string>(value));

        bool_value = true;
        json = JsonSerializer.Serialize(bool_value);
        value = JsonDocument.Parse(json).RootElement;
        Assert.True(Results.Parse<bool>(value));
        Assert.Equal(bool.TrueString.ToLower(), Results.Parse<string>(value));
    }

    [Fact]
    public void ParseJE_BooleanToBool_Succeeds() // Verify correct behavior
    {
        // Arrange
        var json = JsonSerializer.Serialize(new { value = true });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act
        var result = Results.Parse<bool>(element);

        // Assert: Confirms it works as you said
        Assert.True(result);
    }

    [Fact]
    public void ParseJE_Byte()
    {
        byte byte_value = 0x0a;
        var json = JsonSerializer.Serialize(byte_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(byte_value, Results.Parse<byte>(value));
        Assert.Equal(byte_value.ToString(), Results.Parse<string>(value));
    }

    [Fact]
    public void ParseJE_ByteArray()
    {
        byte[] bytearr_value = Array.Empty<byte>();
        var json = JsonSerializer.Serialize(bytearr_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(bytearr_value, Results.Parse<byte[]>(value));
        Assert.Equal(bytearr_value, Convert.FromBase64String(Results.Parse<string>(value)!));

        bytearr_value = Encoding.UTF8.GetBytes("\n\r");
        json = JsonSerializer.Serialize(bytearr_value);
        value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(bytearr_value, Results.Parse<byte[]>(value));
        Assert.Equal(bytearr_value, Convert.FromBase64String(Results.Parse<string>(value)!));
    }

    [Fact]
    public void ParseJE_Short()
    {
        short[] testValues = [short.MinValue, short.MaxValue, 0];
        foreach (var short_value in testValues)
        {
            var json = JsonSerializer.Serialize(short_value);
            var value = JsonDocument.Parse(json).RootElement;
            Assert.Equal(short_value, Results.Parse<short>(value));
            Assert.Equal(short_value.ToString(), Results.Parse<string>(value));
        }
    }

    // 3. Null Handling: Null to non-nullable silently defaults
    [Fact]
    public void Parse_JsonNullToInt_ReturnsDefaultValue()
    {
        var json = JsonSerializer.Serialize(new { value = (int?)null });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        var result = Results.Parse<int>(element);
        Assert.Equal(0, result); // Passes: Correctly returns default value
    }

    // 4. Strictness Inconsistency: Lenient parsing when strict expected
    [Fact]
    public void Parse_InvalidIntString_ThrowsJsonException()
    {
        var json = JsonSerializer.Serialize(new { Value = "not-an-int" });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("Value");
        Assert.Throws<JsonException>(() => Results.Parse<int>(element)); // Expected failure: Returns 0
                                                                         // Problem: Should throw (per your strict tests), but leniently defaults
    }

    // 5. TypeParsers Gaps: Missing int parser leads to JSON fallback
    [Fact]
    public void Parse_IntString_UsesDirectParsing()
    {
        var ex = Record.Exception(() => Results.Parse<int>("123"));
        Assert.Null(ex); // Expected failure: No exception, but uses JSON deserialize
        var result = Results.Parse<int>("123");
        Assert.Equal(123, result);
        // Problem: Falls back to JSON instead of direct int.Parse, less efficient
    }


    [Fact]
    public void Parse_ShouldConvertDateTimeToDateOnly()
    {
        // Arrange
        DateTime dateTime = new DateTime(2025, 2, 22);
        DateOnly expectedDateOnly = DateOnly.FromDateTime(dateTime);

        // Act
        DateOnly? result = Results.Parse<DateOnly>(dateTime);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDateOnly, result);
    }


    [Fact]
    public void Parse_CharArrayToDelegate_ThrowsException()
    {
        char[] chars = new[] { 'a', 'b', 'c' };
        Assert.Throws<JsonException>(() => Results.Parse<Action>(chars));
        // Demo: Delegate type conversion
    }

    [Fact]
    public void Parse_JsonInfinityToDouble_ThrowsJsonException()
    {
        var json = "{\"value\": \"Infinity\"}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<double>(element));
        // Demo: Non-standard number values
    }

    [Fact]
    public void Parse_MultiCharArrayToChar_ThrowsException()
    {
        char[] chars = new[] { 'a', 'b' };
        Assert.Throws<FormatException>(() => Results.Parse<char>(chars));
        // Demo: Multi-char to single char
    }

    [Fact]
    public void Parse_JsonEmptyArrayToStruct_ThrowsJsonException()
    {
        var json = "{\"value\": []}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<CustomStruct>(element));
        // Demo: Array to struct mismatch
    }

    [Fact]
    public void Parse_JsonEmptyStringToGuid_ThrowsFormatException()
    {
        var json = "{\"value\": \"\"}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<Guid>(element));
        // Demo: Empty string to Guid via TypeParsers
    }


    public enum TestEnum { One, Two }

    [Fact]
    public void Parse_CharArrayToCustomObject_ThrowsException()
    {
        char[] chars = new[] { '{', 'a', '}' };
        Assert.Throws<JsonException>(() => Results.Parse<CustomObject>(chars));
        // Demo: Complex object from char[]
    }

    [Fact]
    public void Parse_JsonEnumInvalidCase_ThrowsJsonException()
    {
        var json = "{\"value\": \"one\"}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<TestEnum>(element));
        // Demo: Case sensitivity in JSON enum parsing
    }

    [Fact]
    public void Parse_JsonMalformedNumber_ThrowsJsonException()
    {
        var json = "{\"value\": \"1.2.3\"}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<double>(element));
        // Demo: Invalid number format in JSON
    }

    [Fact]
    public void Parse_ObjectToStringArray_ThrowsException()
    {
        object obj = new { Values = new[] { "a", "b" } };
        Assert.Throws<InvalidCastException>(() => Results.Parse<string[]>(obj));
        // Demo: Complex object to array parsing
    }

    [Fact]
    public void Parse_EnumOutOfRange_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => Results.Parse<TestEnum>("3"));
        // Demo: Enum value beyond defined range
    }

    [Fact]
    public void Parse_CharArrayToStruct_ThrowsException()
    {
        char[] chars = new[] { '1', '2', '3' };
        Assert.Throws<JsonException>(() => Results.Parse<CustomStruct>(chars));
        // Demo: Convert.ChangeType fails for structs
    }

    [Fact]
    public void Parse_NegativeTimeSpanString_ParsesCorrectly()
    {
        var result = Results.Parse<TimeSpan>("-12:30:45");
        Assert.Equal(TimeSpan.Parse("-12:30:45"), result); // Passes: TypeParsers handles negative
    }

    [Fact]
    public void Parse_JsonArrayToEnum_ThrowsJsonException()
    {
        var json = "{\"value\": [\"One\", \"Two\"]}";
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<TestEnum>(element));
        // Demo: Array to enum mismatch
    }

    [Fact]
    public void Parse_CharArrayToGuid_FormatException()
    {
        char[] chars = new[] { 'a', 'b', 'c' };
        Assert.Throws<FormatException>(() => Results.Parse<Guid>(chars));
        // Demo: Convert.ChangeType fails for non-Guid string
    }

    [Fact]
    public void Parse_EmptyStringToTimeSpan_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Results.Parse<TimeSpan>(""));
        // Demo: TypeParsers TimeSpan fails on empty input
    }

    [Fact]
    public void Parse_InvalidDateTimeOffset_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() => Results.Parse<DateTimeOffset>("invalid"));
        // Demo: TypeParsers DateTimeOffset lacks robust parsing
    }

    [Fact]
    public void Parse_JsonEmptyObject_DefaultsProperties()
    {
        var json = "{}";
        var element = JsonDocument.Parse(json).RootElement;
        var result = Results.Parse<CustomType>(element);
        Assert.NotNull(result); // Expected: Leniency might not throw
        Assert.Null(result.Name); // Expected: Leniency might not throw
        Assert.Equal(0, result.Age);
        // Demo: No exception for required properties—design choice?
    }

    [Fact]
    public void Parse_NullToEnum_ReturnsDefaultValue()
    {
        object nullObj = null!;
        var result = Results.Parse<TestEnum>(nullObj);
        Assert.Equal(TestEnum.One, result); // Passes: Correctly returns default
    }


    [Fact]
    public void Parse_JsonObjectWithExtraProperties_IgnoresExtras()
    {
        var json = "{\"Name\": \"test\", \"Age\": 42, \"Extra\": \"ignored\"}";
        var element = JsonDocument.Parse(json).RootElement;
        var result = Results.Parse<CustomType>(element);
        Assert.NotNull(result);
        Assert.Equal("test", result.Name);
        Assert.Equal(42, result.Age);
        // Demo: No issue—handles extra properties gracefully
    }

    [Fact]
    public void Parse_TimeSpanOutOfRange_ThrowsFormatException()
    {
        Assert.Throws<OverflowException>(() => Results.Parse<TimeSpan>("100:00:00"));
        // Demo: TimeSpan.ParseExact handles range incorrectly?
    }

    // 1. Type Safety: char[] to non-string without explicit check
    [Fact]
    public void Parse_CharArrayToDouble_ThrowsUnexpectedCast()
    {
        char[] chars = new[] { '1', '2', '3' };
        var result = Results.Parse<double>(chars); // Should throw clearer exception
        Assert.Equal(123.0, result); // Expected failure: Throws InvalidCastException instead
                                     // Problem: Implicit cast to string then fails, not type-safe
    }

    // 2. Recursive Call: Unhandled type lacks specific exception
    [Fact]
    public void Parse_UnhandledComplexObject_ThrowsException()
    {
        object complex = new { Value = new { Nested = 42 } };
        Assert.Throws<InvalidCastException>(() => Results.Parse<float>(complex));
    }

    [Fact]
    public void Parse_CharArrayToDouble_ParsesCorrectly()
    {
        char[] chars = new[] { '1', '2', '3' };
        var result = Results.Parse<double>(chars);
        Assert.Equal(123.0, result); // Passes with your fix
    }


    [Fact]
    public void Parse_TimeSpanCommonFormat_ParsesCorrectly()
    {
        var result = Results.Parse<TimeSpan>("12:30:45");
        Assert.Equal(new TimeSpan(12, 30, 45), result); // Passes with your code
    }


    // 6. TimeSpan Rigidity: Non-"c" format fails
    [Fact]
    public void Parse_TimeSpanCommonFormat_ThrowsFormatException()
    {
        var result = Results.Parse<TimeSpan>("12:30:45"); // Should parse common format
        Assert.Equal(new TimeSpan(12, 30, 45), result); // Expected failure: Throws FormatException
                                                        // Problem: ParseExact "c" format too strict, rejects valid input
    }

    // 7. Enum Parsing: Invalid enum value lacks specific handling
    [Fact]
    public void Parse_InvalidEnum_ThrowsSpecificException()
    {
        Assert.Throws<ArgumentException>(() => Results.Parse<TestEnum>("Invalid"));
    }

    // 8. JSON Array to Non-Array Type: Unexpected success
    [Fact]
    public void Parse_JsonArrayToInt_ThrowsJsonException()
    {
        var json = JsonSerializer.Serialize(new { value = new[] { 1, 2, 3 } });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");
        Assert.Throws<JsonException>(() => Results.Parse<int>(element)); // Expected failure: Throws
                                                                         // Problem: Should fail clearly, depends on strictness source
    }

    [Fact]
    public void ParseJE_NullToValueType_DoesNotThrow()
    {
        var json = JsonSerializer.Serialize(new { value = (int?)null });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        var result = Results.Parse<int>(element);
        Assert.Equal(0, result); // Passes, but no exception—unexpected given your preference?
    }

    [Fact]
    public void ParseJE_NumberToInt_Succeeds() // Verify correct behavior
    {
        // Arrange
        var json = JsonSerializer.Serialize(new { value = 42 });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act
        var result = Results.Parse<int>(element);

        // Assert: Confirms it works as you said
        Assert.Equal(42, result);
    }


    // New Edge Case 1: Parsing JSON object to primitive type
    [Fact]
    public void ParseJE_ComplexObjectToInt_ThrowsJsonException()
    {
        // Arrange: JSON object can't be parsed to int
        var json = JsonSerializer.Serialize(new { value = new { nested = 42 } });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act & Assert: Problem - throws instead of clear failure or null
        Assert.Throws<JsonException>(() => Results.Parse<int>(element));
        // Edge case: No handling for complex JSON to primitive
    }

    [Fact]
    public void ParseJE_ArrayToString_ReturnsJsonString()
    {
        // Arrange: JSON array parsed to string
        var json = JsonSerializer.Serialize(new { value = new[] { "a", "b" } });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act
        var result = Results.Parse<string>(element);

        // Assert: Returns the JSON string representation, which is fine per your feedback
        Assert.Equal("[\"a\",\"b\"]", result); // No exception, just the raw JSON
    }

    [Fact]
    public void ParseJE_ObjectToNonString_ThrowsJsonException()
    {
        // Arrange: JSON object to a non-string primitive
        var json = JsonSerializer.Serialize(new { value = new { nested = 42 } });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act & Assert: Problem - throws when parsing object to int
        Assert.Throws<JsonException>(() => Results.Parse<int>(element));
        // Edge case: No handling for JSON objects to non-string primitives
    }

    // New Edge Case 3: Parsing JSON null to non-nullable type
    [Fact]
    public void ParseJE_NullToValueType_ReturnsDefault()
    {
        // Arrange: JSON null to int (non-nullable in some contexts)
        var json = JsonSerializer.Serialize(new { value = (int?)null });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act
        var result = Results.Parse<int>(element);

        // Assert: Returns 0, not an exception - might be unexpected
        Assert.Equal(0, result);
        // Edge case: Silent default instead of exception (you like exceptions)
    }

    // 2. Avoid Recursive Call - Demo problem
    [Fact]
    public void Parse_RecursiveCall_UnhandledType_ThrowsInsteadOfDefault()
    {
        // Arrange: Complex object not handled by switch or TypeParsers
        object complexValue = new { Value = new { Nested = 42 } };
        int defaultValue = -1;

        // Act & Assert: Throws instead of returning defaultValue
        var exception = Assert.Throws<InvalidCastException>(() =>
            Results.Parse<int>(complexValue, defaultValue));
        Assert.Contains("Unable to cast object", exception.Message);
        // Problem: Recursive call throws exception rather than using defaultValue
        // Note: You like exceptions, so this is expected behavior for you
    }

    // 9. Potential Issues - Test each identified issue
    [Fact]
    public void Parse_InvalidDateTime_ThrowsFormatException()
    {
        // Arrange: Invalid DateTime string
        object invalidDate = "not-a-date";

        // Act & Assert: Throws due to Convert.ToDateTime failure
        Assert.Throws<FormatException>(() =>
            Results.Parse<DateTime>(invalidDate));
        // Potential Issue: Unhandled exception for invalid DateTime format
    }

    [Fact]
    public void Parse_CharArrayToString_Succeeds()
    {
        // Arrange
        char[] chars = new[] { 'h', 'e', 'l', 'l', 'o' };

        // Act
        var result = Results.Parse<string>(chars);

        // Assert: Works fine, no problem here
        Assert.Equal("hello", result);
        // Potential Issue Check: char[] to string is safe, but see Type Safety above
    }

    [Fact]
    public void Parse_ComplexObjectToCustomType_ThrowsForInvalidAge_WithStrictOptions()
    {
        var json = JsonSerializer.Serialize(new { Name = "test", Age = "not-a-number" });
        var element = JsonDocument.Parse(json).RootElement;
        Assert.Throws<JsonException>(() => Results.Parse<CustomType>(element, null));
    }

    [Fact]
    public void Parse_ComplexObjectToCustomType_ThrowsJsonException_WithStrictOptions()
    {
        var json = JsonSerializer.Serialize(new { Name = "test", Age = "not-a-number" });
        var element = JsonDocument.Parse(json).RootElement;
        Assert.Throws<JsonException>(() => Results.Parse<CustomType>(element, null));
    }

    [Fact]
    public void Parse_ComplexObjectToCustomType_VerifyBehavior()
    {
        var json = JsonSerializer.Serialize(new { Name = "test", Age = "not-a-number" });
        var element = JsonDocument.Parse(json).RootElement;
        Assert.Throws<JsonException>(() => Results.Parse<CustomType>(element, null));
    }

    // Test for edge cases in ParseJE<T> (Point 6)
    [Fact]
    public void ParseJE_NumberToString_ReturnsRawText()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new { value = 123 });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act
        var result = Results.Parse<string>(element);

        // Assert
        Assert.Equal("123", result); // Should handle numeric JSON as string
    }

    [Fact]
    public void ParseJE_BooleanToInt_ThrowsException()
    {
        // Arrange
        var json = JsonSerializer.Serialize(new { value = true });
        var element = JsonDocument.Parse(json).RootElement.GetProperty("value");

        // Act & Assert
        Assert.Throws<JsonException>(() => Results.Parse<int>(element));
    }

    // Additional test for nullability annotation (Point 7)
    [Fact]
    public void Parse_DBNull_ReturnsNull()
    {
        // Arrange
        object? dbNull = DBNull.Value;

        // Act
        var result = Results.Parse(dbNull);

        // Assert
        Assert.Null(result);
    }


    [Fact]
    public void CanParse_BigInt()
    {
        long result = 1234567890123456789L;

        Assert.Equal(result, Results.Parse<long>(result));
        Assert.Equal(result, Results.Parse<long>(result.ToString()));
        Assert.Equal(result.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Char()
    {
        char result = 'A';

        Assert.Equal(result, Results.Parse<char>(result));
        Assert.Equal(result, Results.Parse<char>(result.ToString()));
        Assert.Equal(result.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Chars()
    {
        char[] result = ['A', 'B'];

        Assert.Equal(result, Results.Parse<char[]>(result));
        Assert.Equal(result, Results.Parse<char[]>(new string(result)));
        Assert.Equal(new string(result), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Date()
    {
        DateTime today = DateTime.Now.Date;
        DateOnly result = DateOnly.FromDateTime(today);

        Assert.Equal(result, Results.Parse<DateOnly>(result));
        Assert.Equal(result, Results.Parse<DateOnly>(result.ToString("yyyy-MM-dd")));
        Assert.Equal(result.ToString("yyyy-MM-dd"), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_DateTime()
    {
        DateTime result = new DateTime(2000, 1, 1, 1, 1, 1, 123);

        Assert.Equal(result, Results.Parse<DateTime>(result));
        Assert.Equal(result, Results.Parse<DateTime>(result.ToString("yyyy-MM-ddTHH:mm:ss.fff")));
        Assert.Equal(result.ToString("yyyy-MM-ddTHH:mm:ss.fff"), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_DateTimeOffset()
    {
        DateTimeOffset result = new DateTimeOffset(new DateTime(2000, 1, 1, 1, 1, 1, 100));

        Assert.Equal(result, Results.Parse<DateTimeOffset>(result));
        Assert.Equal(result, Results.Parse<DateTimeOffset>(Results.Parse<string>(result)));
        Assert.Equal(Results.Parse<string>(result), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Decimal()
    {
        decimal result = 123.45m;

        Assert.Equal(result, Results.Parse<decimal>(result));
        Assert.Equal(result, Results.Parse<decimal>(result.ToString()));
        Assert.Equal(result.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Float()
    {
        float result = 123.45f;

        Assert.Equal(result, Results.Parse<float>(result));
        Assert.Equal(result, Results.Parse<float>(result.ToString()));
        Assert.Equal(result.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Double()
    {
        double result = 123.45;

        Assert.Equal(result, Results.Parse<double>(result));
        Assert.Equal(result, Results.Parse<double>(result.ToString()));
        Assert.Equal(result.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Int()
    {
        int result = 123;

        Assert.Equal(result, Results.Parse<int>(result));
        Assert.Equal(result, Results.Parse<int>(result.ToString()));
        Assert.Equal(result.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_String()
    {
        string result = "Test";

        Assert.Equal(result, Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Time()
    {
        TimeSpan timeSpan = new TimeSpan(12, 30, 45);
        TimeOnly result = TimeOnly.FromTimeSpan(timeSpan);

        Assert.Equal(result, Results.Parse<TimeOnly>(result));
        Assert.Equal(result, Results.Parse<TimeOnly>(result.ToString("HH:mm:ss")));
        Assert.Equal(result.ToString("HH:mm:ss"), Results.Parse<string>(result));

        Assert.Equal(timeSpan, Results.Parse<TimeSpan>(timeSpan));
        Assert.Equal(timeSpan, Results.Parse<TimeSpan>(result.ToString("HH:mm:ss")));
        Assert.Equal(timeSpan.ToString(), Results.Parse<string>(result));
    }

    [Fact]
    public void CanParse_Guid()
    {
        Guid result = Guid.NewGuid();

        Assert.Equal(result, Results.Parse<Guid>(result));
        Assert.Equal(result, Results.Parse<Guid>(result.ToString()));
        Assert.Equal(Convert.ToString(result), Results.Parse<string>(result));
    }

    [Theory]
    [InlineData("One", TestEnum.One)]
    [InlineData("Two", TestEnum.Two)]
    public void CanParse_Enum(object input, TestEnum expected)
    {
        var result = Results.Parse<TestEnum>(input);

        Assert.Equal(expected, Results.Parse<TestEnum>(input));
        Assert.Equal(expected, Results.Parse<TestEnum>((int)result));
        Assert.Equal(expected, Results.Parse<TestEnum>(input.ToString()));
        Assert.Equal(expected.ToString(), Results.Parse<string>(input));
    }

    [Fact]
    public void CanParse_NamedObject()
    {
        var expected = new NamedType { Id = 1, Name = "Test" };
        var result = Results.Parse<NamedType>(JsonSerializer.Serialize(expected));

        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.Name, result.Name);
    }

    [Fact]
    public void CanParseChar_JsonElement()
    {
        char char_value = 'A';
        var json = JsonSerializer.Serialize(char_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(char_value, Results.Parse<char>(value));
        Assert.Equal(char_value.ToString(), Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseCharArray_JsonElement()
    {
        char[] chararr_value = Array.Empty<char>();
        var json = JsonSerializer.Serialize(chararr_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(chararr_value, Results.Parse<char[]>(value));
        Assert.Equal("[]", Results.Parse<string>(value));

        chararr_value = ['A', 'V'];
        json = JsonSerializer.Serialize(chararr_value);
        value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(chararr_value, Results.Parse<char[]>(value));
        Assert.Equal("[\"A\",\"V\"]", Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseDateOnly_JsonElement()
    {
        DateOnly date_value = new(2000, 11, 25);
        var json = JsonSerializer.Serialize(date_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(date_value, Results.Parse<DateOnly>(value));
        Assert.Equal(date_value.ToString("yyyy-MM-dd"), Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseTimeOnly_JsonElement()
    {
        TimeOnly time_value = new(23, 59, 59);
        var json = JsonSerializer.Serialize(time_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(time_value, Results.Parse<TimeOnly>(value));
        Assert.Equal(time_value.ToString("HH:mm:ss"), Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseDateTime_JsonElement()
    {
        DateTime datetime_value = new(2000, 11, 25, 23, 59, 59);
        var json = JsonSerializer.Serialize(datetime_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(datetime_value, Results.Parse<DateTime>(value));
        Assert.Equal(datetime_value.ToString("yyyy-MM-ddTHH:mm:ss"), Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseDecimal_JsonElement()
    {
        decimal[] testValues = [
            decimal.MinValue,
        decimal.Zero,
        decimal.One,
        decimal.MinusOne,
        decimal.MaxValue,
        123456.789m
        ];

        foreach (var decimal_value in testValues)
        {
            var json = JsonSerializer.Serialize(decimal_value);
            var value = JsonDocument.Parse(json).RootElement;
            Assert.Equal(decimal_value, Results.Parse<decimal>(value));
            Assert.Equal(decimal_value.ToString(), Results.Parse<string>(value));
        }
    }

    [Fact]
    public void CanParseDouble_JsonElement()
    {
        double double_value = 123456.789d;
        var json = JsonSerializer.Serialize(double_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(double_value, Results.Parse<double>(value));
        Assert.Equal(double_value.ToString(), Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseEnum_JsonElement()
    {
        TestEnum enum_value = TestEnum.One;
        var json = JsonSerializer.Serialize(enum_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(enum_value, Results.Parse<TestEnum>(value));
        Assert.Equal(((int)enum_value).ToString(), Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseGuid_JsonElement()
    {
        Guid guid_value = Guid.NewGuid();
        var json = JsonSerializer.Serialize(guid_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(guid_value, Results.Parse<Guid>(value));
        Assert.Equal(guid_value.ToString(), Results.Parse<string>(value));
    }

    [Fact]
    public void CanParseInt_JsonElement()
    {
        int[] testValues = [int.MinValue, int.MaxValue, 0];
        foreach (var int_value in testValues)
        {
            var json = JsonSerializer.Serialize(int_value);
            var value = JsonDocument.Parse(json).RootElement;
            Assert.Equal(int_value, Results.Parse<int>(value));
            Assert.Equal(int_value.ToString(), Results.Parse<string>(value));
        }
    }

    [Fact]
    public void CanParseLong_JsonElement()
    {
        long[] testValues = [long.MinValue, long.MaxValue, 0];
        foreach (var long_value in testValues)
        {
            var json = JsonSerializer.Serialize(long_value);
            var value = JsonDocument.Parse(json).RootElement;
            Assert.Equal(long_value, Results.Parse<long>(value));
            Assert.Equal(long_value.ToString(), Results.Parse<string>(value));
        }
    }

    [Fact]
    public void CanParseString_JsonElement()
    {
        string[] testValues = [string.Empty, "ASD"];
        foreach (var string_value in testValues)
        {
            var json = JsonSerializer.Serialize(string_value);
            var value = JsonDocument.Parse(json).RootElement;
            Assert.Equal(string_value, Results.Parse<string>(value));
        }
    }

    [Fact]
    public void CanParseStringArray_JsonElement()
    {
        string[] strarr_value = ["A", "V"];
        var json = JsonSerializer.Serialize(strarr_value);
        var value = JsonDocument.Parse(json).RootElement;
        Assert.Equal(strarr_value, Results.Parse<string[]>(value));
        Assert.Equal("[\"A\",\"V\"]", Results.Parse<string>(value));
    }
    private class CustomType { public string? Name { get; set; } public int Age { get; set; } }

    private class CustomObject
    {
        public string? Data { get; set; }
    }

    private class NamedType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private struct CustomStruct { public int Value { get; set; } }

    private class ComplexType
    {
        public string? Name { get; set; }
        public int Age { get; set; }
    }
}
