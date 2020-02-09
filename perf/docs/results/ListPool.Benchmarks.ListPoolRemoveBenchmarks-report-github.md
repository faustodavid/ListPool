``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-PNXGVD : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  InvocationCount=1  
UnrollFactor=1  

```
|   Method |     N |       Mean |     Error |     StdDev |     Median | Ratio | RatioSD | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |------ |-----------:|----------:|-----------:|-----------:|------:|--------:|-----:|------:|------:|------:|----------:|
| ListPool |   100 |   758.2 ns |  63.59 ns |   178.3 ns |   700.0 ns |  0.97 |    0.26 |    1 |     - |     - |     - |         - |
|     List |   100 |   814.3 ns |  72.42 ns |   203.1 ns |   700.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
|          |       |            |           |            |            |       |         |      |       |       |       |           |
|     List |  1000 |   962.8 ns |  64.24 ns |   174.8 ns |   950.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
| ListPool |  1000 | 1,147.3 ns |  95.15 ns |   266.8 ns | 1,100.0 ns |  1.20 |    0.32 |    2 |     - |     - |     - |         - |
|          |       |            |           |            |            |       |         |      |       |       |       |           |
| ListPool | 10000 | 3,992.3 ns | 392.94 ns | 1,101.8 ns | 3,600.0 ns |  1.00 |    0.32 |    1 |     - |     - |     - |         - |
|     List | 10000 | 4,087.9 ns | 292.09 ns |   819.1 ns | 3,800.0 ns |  1.00 |    0.00 |    2 |     - |     - |     - |         - |
