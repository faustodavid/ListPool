``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|               Method |     N |        Mean |     Error |    StdDev | Ratio | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|--------------------- |------ |------------:|----------:|----------:|------:|-----:|-------:|-------:|------:|----------:|
|      ListPool_AsSpan |    50 |    157.8 ns |   0.15 ns |   0.12 ns |  0.52 |    1 | 0.0005 |      - |     - |      40 B |
| ValueListPool_AsSpan |    50 |    185.2 ns |   3.42 ns |   3.20 ns |  0.61 |    2 |      - |      - |     - |         - |
|             ListPool |    50 |    193.7 ns |   0.14 ns |   0.12 ns |  0.64 |    3 | 0.0005 |      - |     - |      40 B |
|        ValueListPool |    50 |    254.5 ns |   4.80 ns |   4.49 ns |  0.84 |    4 |      - |      - |     - |         - |
|                 List |    50 |    301.6 ns |   0.38 ns |   0.30 ns |  1.00 |    5 | 0.0095 |      - |     - |     648 B |
|                      |       |             |           |           |       |      |        |        |       |           |
|             ListPool |  1000 |  2,171.5 ns |   4.65 ns |   3.88 ns |  0.56 |    1 |      - |      - |     - |      40 B |
|      ListPool_AsSpan |  1000 |  2,174.1 ns |  16.57 ns |  15.50 ns |  0.56 |    1 |      - |      - |     - |      40 B |
| ValueListPool_AsSpan |  1000 |  2,325.0 ns |  15.72 ns |  14.70 ns |  0.60 |    2 |      - |      - |     - |         - |
|        ValueListPool |  1000 |  3,286.0 ns |   4.52 ns |   4.01 ns |  0.84 |    3 |      - |      - |     - |         - |
|                 List |  1000 |  3,891.2 ns |   6.06 ns |   5.37 ns |  1.00 |    4 | 0.1221 |      - |     - |    8424 B |
|                      |       |             |           |           |       |      |        |        |       |           |
| ValueListPool_AsSpan | 10000 | 20,424.3 ns |  19.60 ns |  18.34 ns |  0.52 |    1 |      - |      - |     - |         - |
|      ListPool_AsSpan | 10000 | 20,728.9 ns | 185.81 ns | 173.81 ns |  0.52 |    1 |      - |      - |     - |      40 B |
|             ListPool | 10000 | 21,034.3 ns | 231.79 ns | 205.48 ns |  0.53 |    1 |      - |      - |     - |      40 B |
|        ValueListPool | 10000 | 31,486.1 ns |  48.56 ns |  45.42 ns |  0.80 |    2 |      - |      - |     - |         - |
|                 List | 10000 | 39,547.6 ns |  46.05 ns |  38.45 ns |  1.00 |    3 | 1.9531 | 0.1831 |     - |  131400 B |
