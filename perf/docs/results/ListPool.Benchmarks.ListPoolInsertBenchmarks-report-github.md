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
| ListPool |   100 |   547.3 ns |  41.97 ns |   117.7 ns |   500.0 ns |  0.31 |    0.09 |    1 |     - |     - |     - |         - |
|     List |   100 | 1,862.2 ns | 133.96 ns |   373.4 ns | 1,700.0 ns |  1.00 |    0.00 |    2 |     - |     - |     - |     824 B |
|          |       |            |           |            |            |       |         |      |       |       |       |           |
| ListPool |  1000 |   567.0 ns |  56.77 ns |   159.2 ns |   500.0 ns |  0.26 |    0.10 |    1 |     - |     - |     - |         - |
|     List |  1000 | 2,318.4 ns | 264.16 ns |   723.1 ns | 2,100.0 ns |  1.00 |    0.00 |    2 |     - |     - |     - |    8024 B |
|          |       |            |           |            |            |       |         |      |       |       |       |           |
| ListPool | 10000 | 1,854.3 ns | 207.36 ns |   584.9 ns | 1,600.0 ns |  0.24 |    0.10 |    1 |     - |     - |     - |         - |
|     List | 10000 | 8,048.8 ns | 667.10 ns | 1,814.9 ns | 7,700.0 ns |  1.00 |    0.00 |    2 |     - |     - |     - |   80024 B |
