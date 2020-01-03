``` ini

BenchmarkDotNet=v0.12.0, OS=elementary 5.1
Intel Core i7-4702MQ CPU 2.20GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56106, CoreFX 4.700.19.56202), X64 RyuJIT
  Job-OIUWYV : .NET Core 3.1.0 (CoreCLR 4.700.19.56106, CoreFX 4.700.19.56202), X64 RyuJIT

Concurrent=True  Server=True  InvocationCount=1  
UnrollFactor=1  

```
|        Method |    N |     Mean |    Error |    StdDev |    Median | Ratio | RatioSD | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----- |---------:|---------:|----------:|----------:|------:|--------:|-----:|------:|------:|------:|----------:|
|      ListPool |   10 | 14.21 us | 3.778 us | 10.901 us |  8.953 us |  1.89 |    2.60 |    1 |     - |     - |     - |      88 B |
|          Linq |   10 | 15.46 us | 4.082 us | 11.907 us | 11.671 us |  1.00 |    0.00 |    1 |     - |     - |     - |     248 B |
| ListPoolValue |   10 | 17.84 us | 4.111 us | 11.596 us | 14.915 us |  2.04 |    2.30 |    2 |     - |     - |     - |      32 B |
|               |      |          |          |           |           |       |         |      |       |       |       |           |
|      ListPool |   50 | 10.86 us | 2.645 us |  7.372 us |  8.060 us |  1.04 |    0.87 |    1 |     - |     - |     - |      88 B |
| ListPoolValue |   50 | 12.60 us | 2.917 us |  8.370 us |  8.709 us |  1.22 |    0.90 |    1 |     - |     - |     - |      32 B |
|          Linq |   50 | 12.72 us | 2.596 us |  7.533 us |  9.604 us |  1.00 |    0.00 |    1 |     - |     - |     - |     680 B |
|               |      |          |          |           |           |       |         |      |       |       |       |           |
|      ListPool |  100 | 15.03 us | 2.979 us |  8.305 us | 12.022 us |  1.27 |    1.00 |    1 |     - |     - |     - |      88 B |
| ListPoolValue |  100 | 16.30 us | 3.264 us |  9.313 us | 14.026 us |  1.46 |    1.27 |    1 |     - |     - |     - |      32 B |
|          Linq |  100 | 16.95 us | 4.201 us | 12.188 us | 14.171 us |  1.00 |    0.00 |    1 |     - |     - |     - |    1216 B |
|               |      |          |          |           |           |       |         |      |       |       |       |           |
| ListPoolValue | 1000 | 27.80 us | 4.797 us | 13.764 us | 22.904 us |  1.10 |    0.77 |    1 |     - |     - |     - |      32 B |
|      ListPool | 1000 | 29.70 us | 5.124 us | 14.702 us | 26.166 us |  1.19 |    0.93 |    1 |     - |     - |     - |      88 B |
|          Linq | 1000 | 31.31 us | 5.682 us | 16.393 us | 27.160 us |  1.00 |    0.00 |    1 |     - |     - |     - |    8456 B |
