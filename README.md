# ListPool
ListPool, allocation free implementation of IList.

This is a POC of a wrapper over ArrayPool<T> which implements IList or ICollection, still not sure which is the best approach. But main idea is to reduce the complecity of using ArrayPool.

I recommend using similiar approach when working with large arrays. At least bigger than 100 items.

# Benchmarks.

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-8750H CPU 2.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  Job-UIKAVW : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT

Concurrent=True  Server=True  

## Create list


```
|   Method |     N |        Mean |     Error |    StdDev |      Median | Ratio | RatioSD | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|--------- |------ |------------:|----------:|----------:|------------:|------:|--------:|-----:|-------:|-------:|------:|----------:|
|     List |    10 |    13.75 ns |  0.078 ns |  0.073 ns |    13.73 ns |  1.00 |    0.00 |    1 | 0.0014 |      - |     - |      96 B |
| ListPool |    10 |    22.67 ns |  0.254 ns |  0.238 ns |    22.54 ns |  1.65 |    0.02 |    2 |      - |      - |     - |         - |
|          |       |             |           |           |             |       |         |      |        |        |       |           |
| ListPool |   100 |    23.87 ns |  0.493 ns |  0.675 ns |    23.46 ns |  0.55 |    0.02 |    1 |      - |      - |     - |         - |
|     List |   100 |    44.00 ns |  0.439 ns |  0.343 ns |    43.98 ns |  1.00 |    0.00 |    2 | 0.0068 |      - |     - |     456 B |
|          |       |             |           |           |             |       |         |      |        |        |       |           |
| ListPool |  1000 |    22.93 ns |  0.477 ns |  0.603 ns |    22.65 ns |  0.06 |    0.00 |    1 |      - |      - |     - |         - |
|     List |  1000 |   354.87 ns |  7.980 ns |  7.464 ns |   353.90 ns |  1.00 |    0.00 |    2 | 0.0606 |      - |     - |    4056 B |
|          |       |             |           |           |             |       |         |      |        |        |       |           |
| ListPool | 10000 |    24.07 ns |  0.325 ns |  0.304 ns |    23.93 ns | 0.009 |    0.00 |    1 |      - |      - |     - |         - |
|     List | 10000 | 2,827.78 ns | 24.580 ns | 22.992 ns | 2,830.32 ns | 1.000 |    0.00 |    2 | 0.7248 | 0.0229 |     - |   40056 B |
```

## Create list and Insert items

```
|   Method |     N |         Mean |      Error |     StdDev |       Median | Ratio | RatioSD | Rank |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|--------- |------ |-------------:|-----------:|-----------:|-------------:|------:|--------:|-----:|-------:|-------:|------:|----------:|
|     List |    10 |     30.74 ns |   1.017 ns |   2.212 ns |     29.55 ns |  1.00 |    0.00 |    1 | 0.0014 |      - |     - |      96 B |
| ListPool |    10 |     44.14 ns |   0.808 ns |   0.716 ns |     43.80 ns |  1.28 |    0.02 |    2 |      - |      - |     - |         - |
|          |       |              |            |            |              |       |         |      |        |        |       |           |
|     List |   100 |    179.16 ns |   0.642 ns |   0.569 ns |    179.05 ns |  1.00 |    0.00 |    1 | 0.0067 |      - |     - |     456 B |
| ListPool |   100 |    211.16 ns |   0.592 ns |   0.462 ns |    211.07 ns |  1.18 |    0.00 |    2 |      - |      - |     - |         - |
|          |       |              |            |            |              |       |         |      |        |        |       |           |
|     List |  1000 |  1,601.40 ns |   4.301 ns |   3.812 ns |  1,601.62 ns |  1.00 |    0.00 |    1 | 0.0610 |      - |     - |    4056 B |
| ListPool |  1000 |  1,879.24 ns |  37.298 ns |  36.632 ns |  1,877.13 ns |  1.17 |    0.02 |    2 |      - |      - |     - |         - |
|          |       |              |            |            |              |       |         |      |        |        |       |           |
|     List | 10000 | 15,189.33 ns |  32.900 ns |  29.165 ns | 15,184.70 ns |  1.00 |    0.00 |    1 | 0.7172 | 0.0305 |     - |   40056 B |
| ListPool | 10000 | 18,626.51 ns | 370.626 ns | 441.204 ns | 18,468.14 ns |  1.23 |    0.03 |    2 |      - |      - |     - |         - |

```

## Enumerate with foreach

```
|   Method |    N | CapacityFilled |        Mean |    Error |   StdDev | Ratio | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |----- |--------------- |------------:|---------:|---------:|------:|-----:|------:|------:|------:|----------:|
| ListPool | 1000 |            0,1 |    56.14 ns | 0.143 ns | 0.127 ns |  0.30 |    1 |     - |     - |     - |         - |
|     List | 1000 |            0,1 |   188.26 ns | 0.726 ns | 0.644 ns |  1.00 |    2 |     - |     - |     - |         - |
|          |      |                |             |          |          |       |      |       |       |       |           |
| ListPool | 1000 |            0,5 |   258.93 ns | 0.545 ns | 0.510 ns |  0.25 |    1 |     - |     - |     - |         - |
|     List | 1000 |            0,5 | 1,025.01 ns | 2.301 ns | 2.040 ns |  1.00 |    2 |     - |     - |     - |         - |
|          |      |                |             |          |          |       |      |       |       |       |           |
| ListPool | 1000 |            0,8 |   412.65 ns | 1.928 ns | 1.803 ns |  0.25 |    1 |     - |     - |     - |         - |
|     List | 1000 |            0,8 | 1,640.52 ns | 9.632 ns | 8.539 ns |  1.00 |    2 |     - |     - |     - |         - |
|          |      |                |             |          |          |       |      |       |       |       |           |
| ListPool | 1000 |              1 |   515.32 ns | 3.879 ns | 3.239 ns |  0.25 |    1 |     - |     - |     - |         - |
|     List | 1000 |              1 | 2,042.16 ns | 4.994 ns | 4.171 ns |  1.00 |    2 |     - |     - |     - |         - |

```

## Array.ToList()

```
|   Method |    N | CapacityFilled |      Mean |    Error |   StdDev | Ratio | RatioSD | Rank |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------- |----- |--------------- |----------:|---------:|---------:|------:|--------:|-----:|-------:|------:|------:|----------:|
|     List |   10 |              1 |  48.94 ns | 0.216 ns | 0.202 ns |  1.00 |    0.00 |    1 | 0.0014 |     - |     - |      96 B |
| ListPool |   10 |              1 |  62.80 ns | 0.160 ns | 0.142 ns |  1.28 |    0.01 |    2 |      - |     - |     - |         - |
|          |      |                |           |          |          |       |         |      |        |       |       |           |
| ListPool |   50 |              1 |  73.18 ns | 0.967 ns | 0.905 ns |  1.00 |    0.02 |    1 |      - |     - |     - |         - |
|     List |   50 |              1 |  73.44 ns | 0.712 ns | 0.666 ns |  1.00 |    0.00 |    1 | 0.0038 |     - |     - |     256 B |
|          |      |                |           |          |          |       |         |      |        |       |       |           |
| ListPool |  100 |              1 |  90.52 ns | 0.459 ns | 0.383 ns |  0.99 |    0.01 |    1 |      - |     - |     - |         - |
|     List |  100 |              1 |  91.86 ns | 0.577 ns | 0.511 ns |  1.00 |    0.00 |    2 | 0.0068 |     - |     - |     456 B |
|          |      |                |           |          |          |       |         |      |        |       |       |           |
| ListPool | 1000 |              1 | 103.86 ns | 0.910 ns | 0.807 ns |  0.23 |    0.00 |    1 |      - |     - |     - |         - |
|     List | 1000 |              1 | 447.01 ns | 5.094 ns | 4.765 ns |  1.00 |    0.00 |    2 | 0.0610 |     - |     - |    4056 B |
