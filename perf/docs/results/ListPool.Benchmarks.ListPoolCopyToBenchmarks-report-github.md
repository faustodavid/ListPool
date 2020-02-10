``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-PNXGVD : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  InvocationCount=1  
UnrollFactor=1  

```
|   Method |     N |     Mean |    Error |    StdDev |   Median | Ratio | RatioSD | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |------ |---------:|---------:|----------:|---------:|------:|--------:|-----:|------:|------:|------:|----------:|
|     List |   100 | 341.2 ns | 25.07 ns |  67.78 ns | 300.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
| ListPool |   100 | 443.8 ns | 45.18 ns | 125.19 ns | 400.0 ns |  1.37 |    0.52 |    2 |     - |     - |     - |         - |
|          |       |          |          |           |          |       |         |      |       |       |       |           |
| ListPool |  1000 | 230.3 ns | 26.32 ns |  72.93 ns | 200.0 ns |  0.85 |    0.37 |    1 |     - |     - |     - |         - |
|     List |  1000 | 298.9 ns | 41.88 ns | 118.13 ns | 300.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
|          |       |          |          |           |          |       |         |      |       |       |       |           |
| ListPool | 10000 | 333.7 ns | 28.76 ns |  79.69 ns | 300.0 ns |  0.85 |    0.24 |    1 |     - |     - |     - |         - |
|     List | 10000 | 404.3 ns | 29.69 ns |  83.75 ns | 400.0 ns |  1.00 |    0.00 |    2 |     - |     - |     - |         - |
