``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-KUEHLQ : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|               Method |     N |        Mean |    Error |   StdDev | Ratio | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |------ |------------:|---------:|---------:|------:|-----:|-------:|------:|------:|----------:|
| ValueListPool_AsSpan |    50 |    111.9 ns |  0.06 ns |  0.06 ns |  0.56 |    1 |      - |     - |     - |         - |
|      ListPool_AsSpan |    50 |    129.1 ns |  0.35 ns |  0.33 ns |  0.65 |    2 | 0.0005 |     - |     - |      40 B |
|             ListPool |    50 |    129.9 ns |  0.15 ns |  0.12 ns |  0.66 |    2 | 0.0005 |     - |     - |      40 B |
|        ValueListPool |    50 |    175.1 ns |  0.14 ns |  0.13 ns |  0.88 |    3 |      - |     - |     - |         - |
|                 List |    50 |    198.4 ns |  0.40 ns |  0.36 ns |  1.00 |    4 | 0.0038 |     - |     - |     256 B |
|                      |       |             |          |          |       |      |        |       |       |           |
| ValueListPool_AsSpan |  1000 |  1,975.5 ns |  2.14 ns |  2.00 ns |  0.57 |    1 |      - |     - |     - |         - |
|      ListPool_AsSpan |  1000 |  1,994.2 ns |  1.99 ns |  1.86 ns |  0.57 |    1 |      - |     - |     - |      40 B |
|             ListPool |  1000 |  1,995.7 ns |  1.90 ns |  1.58 ns |  0.57 |    1 |      - |     - |     - |      40 B |
|        ValueListPool |  1000 |  3,068.1 ns |  1.84 ns |  1.72 ns |  0.88 |    2 |      - |     - |     - |         - |
|                 List |  1000 |  3,477.5 ns |  6.40 ns |  5.99 ns |  1.00 |    3 | 0.0610 |     - |     - |    4056 B |
|                      |       |             |          |          |       |      |        |       |       |           |
|      ListPool_AsSpan | 10000 | 18,377.7 ns | 98.56 ns | 82.31 ns |  0.53 |    1 |      - |     - |     - |      40 B |
| ValueListPool_AsSpan | 10000 | 18,398.1 ns | 18.81 ns | 16.67 ns |  0.54 |    1 |      - |     - |     - |         - |
|             ListPool | 10000 | 18,419.9 ns | 18.22 ns | 16.15 ns |  0.54 |    1 |      - |     - |     - |      40 B |
|        ValueListPool | 10000 | 29,287.8 ns | 94.27 ns | 78.72 ns |  0.85 |    2 |      - |     - |     - |         - |
|                 List | 10000 | 34,366.7 ns | 44.22 ns | 41.36 ns |  1.00 |    3 | 0.6714 |     - |     - |   40056 B |
