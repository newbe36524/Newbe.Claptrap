``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.348 (1809/October2018Update/Redstone5)
Intel Core i7-4712MQ CPU 2.30GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.100
  [Host] : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT
  Core   : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT

Job=Core  Runtime=Core  InvocationCount=1  
UnrollFactor=1  

```
|               Method | Times |            Mean |             Error |           StdDev |          Median |     Ratio |  RatioSD | Rank | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|--------------------- |------ |----------------:|------------------:|-----------------:|----------------:|----------:|---------:|-----:|------------:|------------:|------------:|--------------------:|
| **ConcurrentDictionary** |     **1** |        **19.83 us** |         **0.8946 us** |         **2.538 us** |        **18.73 us** |      **1.00** |     **0.00** |    **1** |           **-** |           **-** |           **-** |               **544 B** |
|             Claptrap |     1 |     1,707.86 us |        70.1015 us |       204.489 us |     1,692.70 us |     87.88 |    14.40 |    2 |           - |           - |           - |              2216 B |
|                      |       |                 |                   |                  |                 |           |          |      |             |             |             |                     |
| **ConcurrentDictionary** |    **10** |        **32.69 us** |         **1.7097 us** |         **4.987 us** |        **32.16 us** |      **1.00** |     **0.00** |    **1** |           **-** |           **-** |           **-** |              **2440 B** |
|             Claptrap |    10 |     3,461.02 us |        69.2008 us |       121.200 us |     3,438.11 us |    110.93 |    15.84 |    2 |           - |           - |           - |             17576 B |
|                      |       |                 |                   |                  |                 |           |          |      |             |             |             |                     |
| **ConcurrentDictionary** |   **100** |        **93.97 us** |         **2.8302 us** |         **7.983 us** |        **94.07 us** |      **1.00** |     **0.00** |    **1** |           **-** |           **-** |           **-** |             **20864 B** |
|             Claptrap |   100 |    99,193.07 us |    18,935.4532 us |    54,329.365 us |    70,317.65 us |  1,069.73 |   588.42 |    2 |   9000.0000 |   5000.0000 |           - |            170616 B |
|                      |       |                 |                   |                  |                 |           |          |      |             |             |             |                     |
| **ConcurrentDictionary** |  **1000** |       **581.84 us** |        **12.4857 us** |        **35.216 us** |       **582.14 us** |      **1.00** |     **0.00** |    **1** |           **-** |           **-** |           **-** |            **200872 B** |
|             Claptrap |  1000 | 6,584,659.62 us | 1,125,006.2997 us | 3,317,106.457 us | 6,427,711.51 us | 10,686.14 | 5,476.53 |    2 | 185000.0000 |  67000.0000 |           - |           1696048 B |
|                      |       |                 |                   |                  |                 |           |          |      |             |             |             |                     |
| **ConcurrentDictionary** | **10000** |              **NA** |                **NA** |               **NA** |              **NA** |         **?** |        **?** |    **?** |           **-** |           **-** |           **-** |                   **-** |
|             Claptrap | 10000 |              NA |                NA |               NA |              NA |         ? |        ? |    ? |           - |           - |           - |                   - |

Benchmarks with issues:
  ClaptrapBenchmark.ConcurrentDictionary: Core(Runtime=Core, InvocationCount=1, UnrollFactor=1) [Times=10000]
  ClaptrapBenchmark.Claptrap: Core(Runtime=Core, InvocationCount=1, UnrollFactor=1) [Times=10000]
