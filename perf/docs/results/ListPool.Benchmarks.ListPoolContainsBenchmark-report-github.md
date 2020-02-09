``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|   Method |     N |        Mean |    Error |   StdDev | Ratio | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |------ |------------:|---------:|---------:|------:|-----:|------:|------:|------:|----------:|
| ListPool |   100 |    13.38 ns | 0.007 ns | 0.007 ns |  1.00 |    1 |     - |     - |     - |         - |
|     List |   100 |    13.40 ns | 0.027 ns | 0.023 ns |  1.00 |    1 |     - |     - |     - |         - |
|          |       |             |          |          |       |      |       |       |       |           |
|     List |  1000 |   106.33 ns | 0.060 ns | 0.057 ns |  1.00 |    1 |     - |     - |     - |         - |
| ListPool |  1000 |   109.38 ns | 0.491 ns | 0.436 ns |  1.03 |    2 |     - |     - |     - |         - |
|          |       |             |          |          |       |      |       |       |       |           |
|     List | 10000 | 1,035.89 ns | 1.195 ns | 1.060 ns |  1.00 |    1 |     - |     - |     - |         - |
| ListPool | 10000 | 1,075.61 ns | 0.975 ns | 0.912 ns |  1.04 |    2 |     - |     - |     - |         - |
