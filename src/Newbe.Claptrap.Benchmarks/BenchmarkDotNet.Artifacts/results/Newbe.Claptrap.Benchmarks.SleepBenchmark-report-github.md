``` ini

BenchmarkDotNet=v0.11.4, OS=Windows 10.0.17763.348 (1809/October2018Update/Redstone5)
Intel Core i7-4712MQ CPU 2.30GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.2.100
  [Host] : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT
  Core   : .NET Core 2.2.0 (CoreCLR 4.6.27110.04, CoreFX 4.6.27110.04), 64bit RyuJIT

Job=Core  Runtime=Core  

```
|                  Method | Milliseconds |        Mean |     Error |    StdDev | Ratio | Rank | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|------------------------ |------------- |------------:|----------:|----------:|------:|-----:|------------:|------------:|------------:|--------------------:|
|   **&#39;Sleep by Task.Delay&#39;** |           **10** |    **15.61 ms** | **0.0831 ms** | **0.0778 ms** |  **1.47** |    **2** |           **-** |           **-** |           **-** |               **384 B** |
| &#39;Sleep by Thread.Sleep&#39; |           10 |    10.60 ms | 0.0669 ms | 0.0626 ms |  1.00 |    1 |           - |           - |           - |                   - |
|                         |              |             |           |           |       |      |             |             |             |                     |
|   **&#39;Sleep by Task.Delay&#39;** |          **100** |   **108.70 ms** | **0.5573 ms** | **0.4351 ms** |  **1.08** |    **2** |           **-** |           **-** |           **-** |               **384 B** |
| &#39;Sleep by Thread.Sleep&#39; |          100 |   100.54 ms | 0.1604 ms | 0.1500 ms |  1.00 |    1 |           - |           - |           - |                   - |
|                         |              |             |           |           |       |      |             |             |             |                     |
|   **&#39;Sleep by Task.Delay&#39;** |         **1000** | **1,000.58 ms** | **0.5901 ms** | **0.4928 ms** |  **1.00** |    **1** |           **-** |           **-** |           **-** |               **384 B** |
| &#39;Sleep by Thread.Sleep&#39; |         1000 | 1,000.41 ms | 0.2781 ms | 0.2601 ms |  1.00 |    1 |           - |           - |           - |                   - |
|                         |              |             |           |           |       |      |             |             |             |                     |
|   **&#39;Sleep by Task.Delay&#39;** |         **5000** | **5,007.15 ms** | **6.7455 ms** | **6.3097 ms** |  **1.00** |    **1** |           **-** |           **-** |           **-** |               **384 B** |
| &#39;Sleep by Thread.Sleep&#39; |         5000 | 5,000.43 ms | 0.3371 ms | 0.3154 ms |  1.00 |    1 |           - |           - |           - |                   - |
