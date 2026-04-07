using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;

public static class Extens
{
    // Rename your current implementations
    public static bool IsName_V1(string? text) => Extens1.IsName(text);
    public static bool IsName_V2(string? text) => Extens2.IsName(text);
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class IsNameBenchmarks
{
    private readonly List<string?> _testCases = new()
{
    // -----------------------------
    // 1-part names
    // -----------------------------
    "_",
    "a",
    "A",
    "_1",
    "a1",
    "A1",
    "[ ]",
    "[1]",
    "[A 1]",

    // -----------------------------
    // 2-part names
    // -----------------------------
    "_.A",
    "a.A",
    "A.A",
    "_1.A",
    "a1.A",
    "A1.A",
    "[ ].A",
    "[1].A",
    "dbo._",
    "dbo.a",
    "dbo.A",
    "dbo._1",
    "dbo.a1",
    "dbo.A1",
    "dbo.[ ]",
    "dbo.[1]",

    // -----------------------------
    // 3-part names
    // -----------------------------
    "srv.dbo.Tbl",
    "SRV.DB.TBL",
    "[srv].[dbo].[tbl]",
    "[ ].[1].[A 1]"
};

    [GlobalSetup]
    public void CheckCorrectness()
    {
        foreach (var text in _testCases)
        {
            bool v1 = Extens.IsName_V1(text);
            bool v2 = Extens.IsName_V2(text);

            if (v1 != v2)
                Console.WriteLine($"❌ Mismatch on '{text}' → V1={v1}, V2={v2}");
        }
        Console.WriteLine("Correctness check completed.");
    }

    [Benchmark(Baseline = true, Description = "Version 1")]
    [BenchmarkCategory("Performance")]
    public void Version1()
    {
        foreach (var text in _testCases)
            _ = Extens.IsName_V1(text);
    }

    [Benchmark(Description = "Version 2")]
    [BenchmarkCategory("Performance")]
    public void Version2()
    {
        foreach (var text in _testCases)
            _ = Extens.IsName_V2(text);
    }

    // Optional: My optimized version (if you want to compare)
    //[Benchmark(Description = "My Optimized Version")]
    //public void MyVersion()
    //{
    //    foreach (var text in _testCases)
    //        _ = Extens.IsName(text);   // your best version
    //}
}
