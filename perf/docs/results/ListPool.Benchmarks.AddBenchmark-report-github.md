``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.200-preview-014883
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  Job-JNJWYT : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Concurrent=True  Server=True  

```
|                  Method |    N |       Mean |     Error |    StdDev | Ratio | RatioSD | Rank |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------------------------ |----- |-----------:|----------:|----------:|------:|--------:|-----:|--------:|--------:|--------:|----------:|
|     PooledAdd_ValueType |  256 |   4.313 us | 0.0143 us | 0.0127 us |  0.63 |    0.00 |    1 |       - |       - |       - |      40 B |
|       ListAdd_ValueType |  256 |   6.846 us | 0.0170 us | 0.0159 us |  1.00 |    0.00 |    2 |  0.5112 |  0.0153 |       - |   33048 B |
| PooledAdd_ReferenceType |  256 |   7.311 us | 0.0176 us | 0.0138 us |  1.07 |    0.00 |    3 |       - |       - |       - |      40 B |
|   ListAdd_ReferenceType |  256 |  12.163 us | 0.0760 us | 0.0673 us |  1.78 |    0.01 |    4 |  1.0376 |  0.0458 |       - |   65800 B |
|                         |      |            |           |           |       |         |      |         |         |         |           |
|     PooledAdd_ValueType |  512 |   8.273 us | 0.0109 us | 0.0102 us |  0.63 |    0.00 |    1 |       - |       - |       - |      40 B |
|       ListAdd_ValueType |  512 |  13.236 us | 0.0277 us | 0.0246 us |  1.00 |    0.00 |    2 |  1.0071 |  0.0305 |       - |   65840 B |
| PooledAdd_ReferenceType |  512 |  14.208 us | 0.0493 us | 0.0385 us |  1.07 |    0.00 |    3 |       - |       - |       - |      40 B |
|   ListAdd_ReferenceType |  512 |  22.888 us | 0.1202 us | 0.1124 us |  1.73 |    0.01 |    4 |  2.1362 |  0.1526 |       - |  131360 B |
|                         |      |            |           |           |       |         |      |         |         |         |           |
|     PooledAdd_ValueType | 2048 |  32.718 us | 0.1017 us | 0.0901 us |  0.30 |    0.00 |    1 |       - |       - |       - |      40 B |
| PooledAdd_ReferenceType | 2048 |  59.384 us | 1.1870 us | 1.3194 us |  0.54 |    0.01 |    2 |       - |       - |       - |      40 B |
|       ListAdd_ValueType | 2048 | 109.609 us | 1.4058 us | 1.3150 us |  1.00 |    0.00 |    3 |  7.2021 |  5.7373 |  5.7373 |  262532 B |
|   ListAdd_ReferenceType | 2048 | 267.402 us | 5.1730 us | 6.3529 us |  2.44 |    0.07 |    4 | 18.0664 | 16.6016 | 16.6016 |  524703 B |
