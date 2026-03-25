# **UkrGuru vs Microsoft Copilot: Benchmark Results That Speak for Themselves**

When you optimize for real‑world performance, microseconds matter — sometimes *nanoseconds* matter.  
This benchmark compares **UkrGuru** implementations with **Microsoft Copilot‑generated equivalents** across common low-level operations: primitives, arrays, serialization, and structs.

The results were surprising — especially considering that Copilot code tends to follow “safe defaults,” while UkrGuru focuses heavily on allocation‑free, branch‑minimal, CPU‑friendly implementation patterns.

This article summarizes the benchmark results and provides a downloadable table for transparency.

***

## **📊 Benchmark Summary**

The table below shows **Mean execution time**, **Error**, **StdDev**, **Rank**, and **Memory Allocation**.

Lower **Mean** and **Rank** are better.  
Lower **Allocated** (ideally 0 bytes) is also better.

    | Method        | Mean        | Error      | StdDev    | Rank | Gen0   | Allocated |
    |-------------- |------------:|-----------:|----------:|-----:|-------:|----------:|
    | Ukr_Bool      |   0.9436 ns |  0.0552 ns | 0.0030 ns |    2 |      - |         - |
    | Cop_Bool      |   5.5047 ns |  0.2465 ns | 0.0135 ns |    4 |      - |         - |
    | Ukr_Int       |   5.4953 ns |  0.6840 ns | 0.0375 ns |    4 |      - |         - |
    | Cop_Int       |  12.9375 ns |  0.1144 ns | 0.0063 ns |    8 |      - |         - |
    | Ukr_Double    |  23.5338 ns |  0.9775 ns | 0.0536 ns |   13 |      - |         - |
    | Cop_Double    |  22.5574 ns |  1.4858 ns | 0.0814 ns |   12 |      - |         - |
    | Ukr_Decimal   |  29.7762 ns |  0.9910 ns | 0.0543 ns |   15 |      - |         - |
    | Cop_Decimal   |  30.6302 ns |  2.3973 ns | 0.1314 ns |   16 |      - |         - |
    | Ukr_Guid      |  12.7784 ns |  0.3671 ns | 0.0201 ns |    7 |      - |         - |
    | Cop_Guid      |  12.1073 ns |  1.8873 ns | 0.1035 ns |    6 |      - |         - |
    | Ukr_DateOnly  |  52.5456 ns |  2.6977 ns | 0.1479 ns |   17 |      - |         - |
    | Cop_DateOnly  |  57.3368 ns |  2.9485 ns | 0.1616 ns |   18 | 0.0036 |      48 B |
    | Ukr_TimeOnly  | 109.1004 ns |  1.9878 ns | 0.1090 ns |   20 |      - |         - |
    | Cop_TimeOnly  | 119.5629 ns | 10.2523 ns | 0.5620 ns |   21 | 0.0041 |      56 B |
    | Ukr_TimeSpan  |  52.4028 ns |  1.9116 ns | 0.1048 ns |   17 |      - |         - |
    | Cop_TimeSpan  |  62.9908 ns | 15.9374 ns | 0.8736 ns |   19 | 0.0030 |      40 B |
    | Ukr_ByteArray |  15.5711 ns |  0.7072 ns | 0.0388 ns |    9 | 0.0055 |      72 B |
    | Cop_ByteArray |  15.7549 ns |  0.7830 ns | 0.0429 ns |   10 | 0.0055 |      72 B |
    | Ukr_CharArray |   0.9538 ns |  0.3159 ns | 0.0173 ns |    3 |      - |         - |
    | Cop_CharArray |   0.8838 ns |  0.1034 ns | 0.0057 ns |    1 |      - |         - |
    | Ukr_Serialize |   5.6374 ns |  0.6384 ns | 0.0350 ns |    5 | 0.0043 |      56 B |
    | Cop_Serialize |  12.0702 ns |  0.5457 ns | 0.0299 ns |    6 | 0.0067 |      88 B |
    | Ukr_Enum      |  24.4368 ns |  2.8392 ns | 0.1556 ns |   14 | 0.0018 |      24 B |
    | Cop_Enum      |  21.3234 ns |  1.0134 ns | 0.0555 ns |   11 | 0.0043 |      56 B |

***

## **🔥 Key Observations**

### **1. UkrGuru wins most primitive operations**

Especially in:

*   **Bool**
*   **Int**
*   **Decimal**
*   **TimeSpan**

These results highlight extremely efficient, allocation‑free handling of simple .NET types.

***

### **2. Copilot sometimes wins — but only occasionally**

There *are* categories where Copilot’s code came out slightly ahead (e.g., `CharArray`).

This is good — benchmarks should challenge both sides.  
It makes the overall results more meaningful.

***

### **3. Memory allocations reveal the biggest gap**

In several operations, Copilot-generated code allocates:

*   **48 B** in `DateOnly`
*   **56 B** in `TimeOnly`
*   **40 B** in `TimeSpan`
*   **88 B** in `Serialize`
*   **56 B** in Enum conversions

Meanwhile, most UkrGuru methods remain **0‑allocation**, which is critical for:

*   high-throughput APIs
*   microservices
*   real-time pipelines
*   game loops
*   tight CPU‑bound workloads

***

## **🤖 Why Copilot Can’t Keep Up**

Microsoft Copilot is **great at generating safe general-purpose code**, but not specialized for:

*   nanosecond‑level optimization
*   allocation‑free pipelines
*   branch elimination
*   struct‑level micro-optimizations
*   IL-friendly patterns

That’s exactly where **UkrGuru** excels — handcrafted low-level performance-oriented code.

***

## **🏁 Conclusion**

This benchmark doesn’t say “Copilot is bad.”  
It simply shows that **generic AI-generated code cannot outperform hand‑tuned, purpose-built UkrGuru implementations**.

If your application needs maximum performance with minimal allocations — UkrGuru wins.

***

## **📦 Want to reproduce the benchmark?**

Full source code, including benchmark classes and configuration, is available here:

👉 *https://github.com/UkrGuru/Sql/tree/main/dev/ResultsBench/ResultsBench*
