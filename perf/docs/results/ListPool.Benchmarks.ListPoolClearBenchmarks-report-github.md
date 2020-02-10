``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|   Method |     N |      Mean |     Error |    StdDev |    Median | Ratio | RatioSD | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |------ |----------:|----------:|----------:|----------:|------:|--------:|-----:|------:|------:|------:|----------:|
| ListPool |   100 | 0.0043 ns | 0.0017 ns | 0.0016 ns | 0.0045 ns |  0.48 |    0.20 |    1 |     - |     - |     - |         - |
|     List |   100 | 0.0091 ns | 0.0010 ns | 0.0009 ns | 0.0092 ns |  1.00 |    0.00 |    2 |     - |     - |     - |         - |
|          |       |           |           |           |           |       |         |      |       |       |       |           |
| ListPool |  1000 | 0.0018 ns | 0.0036 ns | 0.0034 ns | 0.0000 ns |  0.25 |    0.44 |    1 |     - |     - |     - |         - |
|     List |  1000 | 0.0092 ns | 0.0046 ns | 0.0038 ns | 0.0083 ns |  1.00 |    0.00 |    2 |     - |     - |     - |         - |
|          |       |           |           |           |           |       |         |      |       |       |       |           |
| ListPool | 10000 | 0.0044 ns | 0.0022 ns | 0.0021 ns | 0.0049 ns |  0.12 |    0.06 |    1 |     - |     - |     - |         - |
|     List | 10000 | 0.0369 ns | 0.0008 ns | 0.0007 ns | 0.0367 ns |  1.00 |    0.00 |    2 |     - |     - |     - |         - |
