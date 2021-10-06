// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class TimerTest
{
    [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
    public static extern uint TimeBeginPeriod(uint uMilliseconds);

    [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
    public static extern uint TimeEndPeriod(uint uMilliseconds);

    public int[] IntArray = default!;
    public int[] SourceArray = default!;

    public TimerTest()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public long GetTimestamp()
    {
        return Stopwatch.GetTimestamp();
    }

    [Benchmark]
    public void TimeBeginEndPeriod()
    {
        try
        {
            TimeBeginPeriod(1);
            TimeEndPeriod(1);
        }
        catch
        {
        }
    }
}
