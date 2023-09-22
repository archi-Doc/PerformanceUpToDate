// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CalcTest
{
    private const int N = 100;
    private long[] longData = new long[N];
    private Int128[] int128Data = new Int128[N];
    private decimal[] decimalData = new decimal[N];

    public CalcTest()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
        for (var i = 0; i < N; i++)
        {
            this.longData[i] = i + 1;
            this.int128Data[i] = i + 1;
            this.decimalData[i] = i + 1;
        }

        var totalLong = this.TotalLong();
        var totalInt128 = this.TotalInt128();
        var totalDecimal = this.TotalDecimal();
        var ratioLong = this.RatioLong();
        var ratioInt128 = this.RatioInt128();
        var ratioDecimal = this.RatioDecimal();
    }

    [Benchmark]
    public long TotalLong()
        => this.longData.Sum();

    [Benchmark]
    public Int128 TotalInt128()
    {
        var total = default(Int128);
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            total += this.int128Data[i];
        }

        return total;
    }

    [Benchmark]
    public decimal TotalDecimal()
        => this.decimalData.Sum();

    [Benchmark]
    public double RatioLong()
    {
        var ratio = 0d;
        for (var i = 0; i < this.longData.Length; i++)
        {
            ratio += (double)this.longData[i] / (double)this.longData[N - 1 - i];
        }

        return ratio;
    }

    [Benchmark]
    public double RatioInt128()
    {
        var ratio = 0d;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            ratio += (double)this.int128Data[i] / (double)this.int128Data[N - 1 - i];
        }

        return ratio;
    }

    [Benchmark]
    public double RatioInt128B()
    {
        var ratio = 0d;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            ratio += (double)(long)this.int128Data[i] / (double)(long)this.int128Data[N - 1 - i];
        }

        return ratio;
    }

    [Benchmark]
    public double RatioDecimal()
    {
        var ratio = 0d;
        for (var i = 0; i < this.decimalData.Length; i++)
        {
            ratio += (double)this.decimalData[i] / (double)this.decimalData[N - 1 - i];
        }

        return ratio;
    }
}
