``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|   Method |     N |        Mean |     Error |   StdDev | Ratio | RatioSD | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|--------- |------ |------------:|----------:|---------:|------:|--------:|-----:|-------:|-------:|------:|----------:|
|     Linq |   100 |    187.9 ns |   0.40 ns |  0.36 ns |  1.00 |    0.00 |    1 | 0.0074 |      - |     - |     496 B |
| ListPool |   100 |    572.0 ns |   0.54 ns |  0.50 ns |  3.04 |    0.01 |    2 | 0.0010 |      - |     - |      80 B |
|          |       |             |           |          |       |         |      |        |        |       |           |
|     Linq |  1000 |  1,520.2 ns |   4.26 ns |  3.56 ns |  1.00 |    0.00 |    1 | 0.0610 |      - |     - |    4096 B |
| ListPool |  1000 |  4,207.0 ns |  17.39 ns | 14.52 ns |  2.77 |    0.01 |    2 |      - |      - |     - |      80 B |
|          |       |             |           |          |       |         |      |        |        |       |           |
|     Linq | 10000 | 14,419.0 ns | 105.10 ns | 87.76 ns |  1.00 |    0.00 |    1 | 0.7172 | 0.0305 |     - |   40096 B |
| ListPool | 10000 | 43,642.5 ns |  50.96 ns | 45.17 ns |  3.03 |    0.02 |    2 |      - |      - |     - |      80 B |
