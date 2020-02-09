``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-FICKMC : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|           Method |     N |       Mean |     Error |    StdDev | Ratio | RatioSD | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|----------------- |------ |-----------:|----------:|----------:|------:|--------:|-----:|-------:|-------:|------:|----------:|
|     List_Spreads |   100 |   1.310 us | 0.0240 us | 0.0225 us |  0.62 |    0.01 |    1 | 0.0172 |      - |     - |    1184 B |
| ListPool_Spreads |   100 |   1.418 us | 0.0268 us | 0.0237 us |  0.67 |    0.01 |    2 |      - |      - |     - |      40 B |
|             List |   100 |   2.127 us | 0.0067 us | 0.0063 us |  1.00 |    0.00 |    3 | 0.0153 |      - |     - |    1184 B |
|         ListPool |   100 |   2.243 us | 0.0306 us | 0.0286 us |  1.05 |    0.01 |    4 |      - |      - |     - |      40 B |
|                  |       |            |           |           |       |         |      |        |        |       |           |
|     List_Spreads |  1000 |  10.953 us | 0.1397 us | 0.1307 us |  0.57 |    0.01 |    1 | 0.1221 |      - |     - |    8424 B |
| ListPool_Spreads |  1000 |  12.600 us | 0.1434 us | 0.1342 us |  0.65 |    0.01 |    2 |      - |      - |     - |      40 B |
|             List |  1000 |  19.366 us | 0.0546 us | 0.0456 us |  1.00 |    0.00 |    3 | 0.1221 |      - |     - |    8424 B |
|         ListPool |  1000 |  21.853 us | 0.0221 us | 0.0196 us |  1.13 |    0.00 |    4 |      - |      - |     - |      40 B |
|                  |       |            |           |           |       |         |      |        |        |       |           |
|     List_Spreads | 10000 | 121.053 us | 0.2503 us | 0.2341 us |  0.59 |    0.00 |    1 | 2.0752 | 0.1221 |     - |  131400 B |
| ListPool_Spreads | 10000 | 129.697 us | 2.0257 us | 1.7957 us |  0.63 |    0.01 |    2 |      - |      - |     - |      42 B |
|             List | 10000 | 204.835 us | 0.1859 us | 0.1552 us |  1.00 |    0.00 |    3 | 1.9531 |      - |     - |  131401 B |
|         ListPool | 10000 | 219.071 us | 4.2278 us | 3.9546 us |  1.07 |    0.02 |    4 |      - |      - |     - |      40 B |
