``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|               Method |     N |         Mean |     Error |    StdDev | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |------ |-------------:|----------:|----------:|------:|--------:|-----:|-------:|------:|------:|----------:|
|             ListPool |    48 |     125.3 ns |   0.14 ns |   0.12 ns |  0.67 |    0.00 |    1 | 0.0005 |     - |     - |      40 B |
|        ValueListPool |    48 |     163.0 ns |   0.09 ns |   0.08 ns |  0.87 |    0.00 |    2 |      - |     - |     - |         - |
|                 List |    48 |     186.3 ns |   0.26 ns |   0.24 ns |  1.00 |    0.00 |    3 | 0.0036 |     - |     - |     248 B |
|                      |       |              |           |           |       |         |      |        |       |       |           |
|             ListPool |  1024 |   1,983.1 ns |   3.79 ns |   3.36 ns |  0.56 |    0.00 |    1 |      - |     - |     - |      40 B |
|        ValueListPool |  1024 |   3,000.2 ns |   2.08 ns |   1.95 ns |  0.84 |    0.00 |    2 |      - |     - |     - |         - |
|                 List |  1024 |   3,555.5 ns |   4.45 ns |   3.72 ns |  1.00 |    0.00 |    3 | 0.0610 |     - |     - |    4152 B |
|                      |       |              |           |           |       |         |      |        |       |       |           |
|             ListPool | 10240 |  19,599.4 ns |  15.04 ns |  14.07 ns |  0.56 |    0.00 |    1 |      - |     - |     - |      40 B |
|        ValueListPool | 10240 |  28,551.9 ns |  18.21 ns |  17.04 ns |  0.82 |    0.00 |    2 |      - |     - |     - |         - |
|                 List | 10240 |  34,734.7 ns |  70.42 ns |  65.87 ns |  1.00 |    0.00 |    3 | 0.6714 |     - |     - |   41016 B |
