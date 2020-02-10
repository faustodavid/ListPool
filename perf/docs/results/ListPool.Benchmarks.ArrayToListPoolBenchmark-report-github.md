``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|        Method |     N |        Mean |     Error |    StdDev | Ratio | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|-------------- |------ |------------:|----------:|----------:|------:|-----:|-------:|-------:|------:|----------:|
| ValueListPool |   100 |    15.79 ns |  0.041 ns |  0.038 ns |  0.20 |    1 |      - |      - |     - |         - |
|      ListPool |   100 |    78.58 ns |  0.373 ns |  0.331 ns |  0.97 |    2 | 0.0006 |      - |     - |      40 B |
|          List |   100 |    80.89 ns |  0.403 ns |  0.377 ns |  1.00 |    3 | 0.0068 |      - |     - |     456 B |
|               |       |             |           |           |       |      |        |        |       |           |
| ValueListPool |  1000 |    16.44 ns |  0.029 ns |  0.027 ns |  0.05 |    1 |      - |      - |     - |         - |
|      ListPool |  1000 |   116.99 ns |  0.120 ns |  0.100 ns |  0.33 |    2 | 0.0006 |      - |     - |      40 B |
|          List |  1000 |   352.43 ns |  2.859 ns |  2.674 ns |  1.00 |    3 | 0.0606 |      - |     - |    4056 B |
|               |       |             |           |           |       |      |        |        |       |           |
| ValueListPool | 10000 |    15.79 ns |  0.043 ns |  0.041 ns | 0.005 |    1 |      - |      - |     - |         - |
|      ListPool | 10000 |   960.63 ns |  1.661 ns |  1.553 ns | 0.294 |    2 |      - |      - |     - |      40 B |
|          List | 10000 | 3,271.11 ns | 11.879 ns | 11.112 ns | 1.000 |    3 | 0.7248 | 0.0305 |     - |   40056 B |
