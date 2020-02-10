``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-PNXGVD : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  InvocationCount=1  
UnrollFactor=1  

```
|   Method |     N |       Mean |     Error |    StdDev |     Median | Ratio | RatioSD | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |------ |-----------:|----------:|----------:|-----------:|------:|--------:|-----:|------:|------:|------:|----------:|
|     List |   100 |   328.9 ns |  43.64 ns | 126.62 ns |   300.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
| ListPool |   100 |   487.9 ns |  31.63 ns |  86.59 ns |   450.0 ns |  1.68 |    0.68 |    2 |     - |     - |     - |         - |
|          |       |            |           |           |            |       |         |      |       |       |       |           |
|     List |  1000 |   431.9 ns |  52.33 ns | 146.73 ns |   400.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
| ListPool |  1000 |   620.8 ns |  85.92 ns | 247.90 ns |   550.0 ns |  1.60 |    0.79 |    2 |     - |     - |     - |         - |
|          |       |            |           |           |            |       |         |      |       |       |       |           |
|     List | 10000 | 1,548.3 ns | 251.04 ns | 695.62 ns | 1,300.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
| ListPool | 10000 | 1,711.0 ns | 283.53 ns | 795.05 ns | 1,500.0 ns |  1.30 |    0.80 |    1 |     - |     - |     - |         - |
