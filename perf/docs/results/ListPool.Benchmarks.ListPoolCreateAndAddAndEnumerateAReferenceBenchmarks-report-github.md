``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|   Method |     N |        Mean |     Error |   StdDev | Ratio | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|--------- |------ |------------:|----------:|---------:|------:|-----:|-------:|-------:|------:|----------:|
| ListPool |    48 |    256.5 ns |   0.43 ns |  0.38 ns |  0.85 |    1 | 0.0005 |      - |     - |      40 B |
|     List |    48 |    300.4 ns |   1.73 ns |  1.62 ns |  1.00 |    2 | 0.0062 |      - |     - |     440 B |
|          |       |             |           |          |       |      |        |        |       |           |
| ListPool |  1024 |  4,378.6 ns |   4.78 ns |  3.73 ns |  0.76 |    1 |      - |      - |     - |      40 B |
|     List |  1024 |  5,776.0 ns |  10.84 ns |  8.46 ns |  1.00 |    2 | 0.1221 |      - |     - |    8248 B |
|          |       |             |           |          |       |      |        |        |       |           |
| ListPool | 10240 | 43,080.9 ns |  36.88 ns | 34.50 ns |  0.77 |    1 |      - |      - |     - |      40 B |
|     List | 10240 | 56,172.6 ns | 115.54 ns | 96.48 ns |  1.00 |    2 | 1.2817 | 0.1221 |     - |   81976 B |
