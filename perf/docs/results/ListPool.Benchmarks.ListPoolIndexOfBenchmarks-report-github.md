``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-PNXGVD : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  InvocationCount=1  
UnrollFactor=1  

```
|   Method |     N |       Mean |     Error |   StdDev |     Median | Ratio | RatioSD | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |------ |-----------:|----------:|---------:|-----------:|------:|--------:|-----:|------:|------:|------:|----------:|
|     List |   100 |   520.0 ns |  43.42 ns | 121.0 ns |   500.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
| ListPool |   100 |   549.5 ns |  68.09 ns | 193.2 ns |   500.0 ns |  1.12 |    0.49 |    1 |     - |     - |     - |         - |
|          |       |            |           |          |            |       |         |      |       |       |       |           |
| ListPool |  1000 |   615.6 ns |  81.14 ns | 234.1 ns |   500.0 ns |  1.00 |    0.47 |    1 |     - |     - |     - |         - |
|     List |  1000 |   666.3 ns |  64.22 ns | 181.1 ns |   600.0 ns |  1.00 |    0.00 |    1 |     - |     - |     - |         - |
|          |       |            |           |          |            |       |         |      |       |       |       |           |
| ListPool | 10000 | 2,616.1 ns | 189.57 ns | 537.8 ns | 2,500.0 ns |  0.93 |    0.29 |    1 |     - |     - |     - |         - |
|     List | 10000 | 2,972.8 ns | 291.38 ns | 821.8 ns | 2,700.0 ns |  1.00 |    0.00 |    2 |     - |     - |     - |         - |
