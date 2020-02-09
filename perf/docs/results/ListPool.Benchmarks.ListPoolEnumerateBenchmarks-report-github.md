``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|         Method |     N |          Mean |      Error |     StdDev | Ratio | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------- |------ |--------------:|-----------:|-----------:|------:|-----:|------:|------:|------:|----------:|
|       ListPool |    10 |      4.532 ns |  0.0405 ns |  0.0379 ns |  0.21 |    1 |     - |     - |     - |         - |
| ListPoolAsSpan |    10 |      5.661 ns |  0.0177 ns |  0.0157 ns |  0.27 |    2 |     - |     - |     - |         - |
|           List |    10 |     21.232 ns |  0.0431 ns |  0.0403 ns |  1.00 |    3 |     - |     - |     - |         - |
|                |       |               |            |            |       |      |       |       |       |           |
| ListPoolAsSpan |   100 |     43.561 ns |  0.0519 ns |  0.0486 ns |  0.21 |    1 |     - |     - |     - |         - |
|       ListPool |   100 |     44.612 ns |  0.0290 ns |  0.0271 ns |  0.22 |    2 |     - |     - |     - |         - |
|           List |   100 |    203.228 ns |  0.2113 ns |  0.1873 ns |  1.00 |    3 |     - |     - |     - |         - |
|                |       |               |            |            |       |      |       |       |       |           |
|       ListPool |  1000 |    386.262 ns |  0.5595 ns |  0.4960 ns |  0.21 |    1 |     - |     - |     - |         - |
| ListPoolAsSpan |  1000 |    510.027 ns |  0.4755 ns |  0.4447 ns |  0.28 |    2 |     - |     - |     - |         - |
|           List |  1000 |  1,807.256 ns |  1.9395 ns |  1.7194 ns |  1.00 |    3 |     - |     - |     - |         - |
|                |       |               |            |            |       |      |       |       |       |           |
| ListPoolAsSpan | 10000 |  3,724.218 ns |  2.3391 ns |  2.0735 ns |  0.19 |    1 |     - |     - |     - |         - |
|       ListPool | 10000 |  3,822.299 ns | 13.3983 ns | 11.1882 ns |  0.20 |    2 |     - |     - |     - |         - |
|           List | 10000 | 19,332.180 ns | 16.6956 ns | 13.9416 ns |  1.00 |    3 |     - |     - |     - |         - |
