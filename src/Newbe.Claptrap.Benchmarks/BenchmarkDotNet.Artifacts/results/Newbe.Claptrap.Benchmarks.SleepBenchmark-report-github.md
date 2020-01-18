```ini
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon CPU E5-2678 v3 2.50GHz, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT
  Job-HGBTAT : .NET Core 3.1.0 (CoreCLR 4.700.19.56402, CoreFX 4.700.19.56404), X64 RyuJIT

Runtime=.NET Core 3.1  
```

|                  Method | Milliseconds |        Mean |    Error |   StdDev | Ratio | Rank | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------ |------------- |------------:|---------:|---------:|------:|-----:|------:|------:|------:|----------:|
|   'Sleep by Task.Delay' |           10 |    15.59 ms | 0.094 ms | 0.084 ms |  1.46 |    2 |     - |     - |     - |     392 B |
| 'Sleep by Thread.Sleep' |           10 |    10.69 ms | 0.066 ms | 0.062 ms |  1.00 |    1 |     - |     - |     - |      76 B |
|                         |              |             |          |          |       |      |       |       |       |           |
|   'Sleep by Task.Delay' |          100 |   108.94 ms | 0.640 ms | 0.599 ms |  1.08 |    2 |     - |     - |     - |     352 B |
| 'Sleep by Thread.Sleep' |          100 |   100.55 ms | 0.118 ms | 0.110 ms |  1.00 |    1 |     - |     - |     - |     974 B |
|                         |              |             |          |          |       |      |       |       |       |           |
|   'Sleep by Task.Delay' |         1000 | 1,000.67 ms | 0.627 ms | 0.490 ms |  1.00 |    1 |     - |     - |     - |    1480 B |
| 'Sleep by Thread.Sleep' |         1000 | 1,000.56 ms | 0.310 ms | 0.290 ms |  1.00 |    1 |     - |     - |     - |         - |
|                         |              |             |          |          |       |      |       |       |       |           |
|   'Sleep by Task.Delay' |         5000 | 5,003.37 ms | 4.442 ms | 4.155 ms |  1.00 |    1 |     - |     - |     - |     680 B |
| 'Sleep by Thread.Sleep' |         5000 | 5,000.62 ms | 0.328 ms | 0.307 ms |  1.00 |    1 |     - |     - |     - |    1336 B |
