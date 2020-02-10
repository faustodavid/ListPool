``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|        Method |     N |        Mean |    Error |   StdDev |      Median | Ratio | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|-------------- |------ |------------:|---------:|---------:|------------:|------:|-----:|-------:|-------:|------:|----------:|
| ValueListPool |   100 |    28.77 ns | 0.031 ns | 0.024 ns |    28.77 ns |  0.83 |    1 |      - |      - |     - |         - |
|      ListPool |   100 |    30.39 ns | 0.463 ns | 0.433 ns |    30.51 ns |  0.88 |    2 | 0.0006 |      - |     - |      40 B |
|          List |   100 |    34.69 ns | 0.179 ns | 0.167 ns |    34.66 ns |  1.00 |    3 | 0.0068 |      - |     - |     456 B |
|               |       |             |          |          |             |       |      |        |        |       |           |
| ValueListPool |  1000 |    25.80 ns | 0.544 ns | 0.688 ns |    25.36 ns |  0.10 |    1 |      - |      - |     - |         - |
|      ListPool |  1000 |    28.82 ns | 0.072 ns | 0.060 ns |    28.84 ns |  0.11 |    2 | 0.0006 |      - |     - |      40 B |
|          List |  1000 |   261.93 ns | 2.813 ns | 2.631 ns |   261.56 ns |  1.00 |    3 | 0.0606 |      - |     - |    4056 B |
|               |       |             |          |          |             |       |      |        |        |       |           |
| ValueListPool | 10000 |    25.25 ns | 0.017 ns | 0.013 ns |    25.25 ns |  0.01 |    1 |      - |      - |     - |         - |
|      ListPool | 10000 |    28.86 ns | 0.211 ns | 0.165 ns |    28.78 ns |  0.01 |    2 | 0.0006 |      - |     - |      40 B |
|          List | 10000 | 2,111.99 ns | 7.089 ns | 6.631 ns | 2,110.49 ns |  1.00 |    3 | 0.7248 | 0.0267 |     - |   40056 B |
