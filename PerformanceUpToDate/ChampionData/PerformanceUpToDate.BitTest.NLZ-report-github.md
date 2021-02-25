``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-6700K CPU 4.00GHz (Skylake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.103
  [Host]    : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT
  MediumRun : .NET Core 5.0.3 (CoreCLR 5.0.321.7212, CoreFX 5.0.321.7212), X64 RyuJIT

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  

```
|       Method |       N |      Mean |     Error |    StdDev |    Median | Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------- |-------- |----------:|----------:|----------:|----------:|------:|------:|------:|----------:|
| **BitOperation** |      **10** | **0.0000 ns** | **0.0000 ns** | **0.0000 ns** | **0.0000 ns** |     **-** |     **-** |     **-** |         **-** |
|         Log2 |      10 | 1.3112 ns | 0.0313 ns | 0.0459 ns | 1.3523 ns |     - |     - |     - |         - |
|     NoBranch |      10 | 2.7913 ns | 0.0053 ns | 0.0076 ns | 2.7898 ns |     - |     - |     - |         - |
|       Branch |      10 | 1.5535 ns | 0.0039 ns | 0.0056 ns | 1.5544 ns |     - |     - |     - |         - |
|        Union |      10 | 0.3270 ns | 0.0020 ns | 0.0030 ns | 0.3270 ns |     - |     - |     - |         - |
| **BitOperation** |    **1000** | **0.0003 ns** | **0.0006 ns** | **0.0009 ns** | **0.0000 ns** |     **-** |     **-** |     **-** |         **-** |
|         Log2 |    1000 | 2.6967 ns | 0.0382 ns | 0.0536 ns | 2.7318 ns |     - |     - |     - |         - |
|     NoBranch |    1000 | 2.7906 ns | 0.0057 ns | 0.0082 ns | 2.7892 ns |     - |     - |     - |         - |
|       Branch |    1000 | 1.3140 ns | 0.0050 ns | 0.0074 ns | 1.3142 ns |     - |     - |     - |         - |
|        Union |    1000 | 0.3276 ns | 0.0019 ns | 0.0027 ns | 0.3271 ns |     - |     - |     - |         - |
| **BitOperation** | **1000000** | **0.1310 ns** | **0.0890 ns** | **0.1332 ns** | **0.1294 ns** |     **-** |     **-** |     **-** |         **-** |
|         Log2 | 1000000 | 5.8379 ns | 0.0861 ns | 0.1288 ns | 5.8527 ns |     - |     - |     - |         - |
|     NoBranch | 1000000 | 2.7907 ns | 0.0029 ns | 0.0043 ns | 2.7903 ns |     - |     - |     - |         - |
|       Branch | 1000000 | 1.1518 ns | 0.0032 ns | 0.0047 ns | 1.1520 ns |     - |     - |     - |         - |
|        Union | 1000000 | 0.8680 ns | 0.3619 ns | 0.5304 ns | 1.3663 ns |     - |     - |     - |         - |
