// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using CommandLine.Text;

#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate;

internal class CalcClass
{
    private int x = 2;
}

internal readonly struct Int128Struct
{
    public Int128Struct(Int128 value)
    {
    }

    public readonly ulong Lower;
    public readonly ulong Upper;

    // private readonly Int128 value;

    /*[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_lower")]
    static extern ref ulong __lower__(Int128 value);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_upper")]
    static extern ref ulong __upper__(Int128 value);

    public ulong Lower => __lower__(value);

    public ulong Upper => __upper__(value);*/
}

public static class Int128Helper
{
    public static double ToDouble(this Int128 value)
    {
        return (value >> 64) == 0 ? (double)(long)value : (double)value;
    }

    public static double ToDouble2(this Int128 value)
    {
        return Unsafe.As<Int128, Int128Struct>(ref value).Upper == 0 ? (double)(long)value : (double)value;
    }

    public static unsafe double ToDouble3(this Int128 value)
    {
        return Int128.LeadingZeroCount(value) >= 64 ? (double)(long)value : (double)value;
    }
}

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

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "x")]
    static extern ref int CalcClassX(CalcClass value);

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

        var c = new CalcClass();
        var x = CalcClassX(c);

        var i128 = new Int128(123, 456);
        var st = Unsafe.As<Int128, Int128Struct>(ref i128);
        // var higher = Int128Upper(i128);
        // var lower = new Int128Struct(i128).Lower;
    }

    // [Benchmark]
    public long TotalLong()
        => this.longData.Sum();

    // [Benchmark]
    public Int128 TotalInt128()
    {
        var total = default(Int128);
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            total += this.int128Data[i];
        }

        return total;
    }

    // [Benchmark]
    public decimal TotalDecimal()
        => this.decimalData.Sum();

    // [Benchmark]
    public double RatioLong()
    {
        var ratio = 0d;
        for (var i = 0; i < this.longData.Length; i++)
        {
            ratio += (double)this.longData[i] / (double)this.longData[N - 1 - i];
        }

        return ratio;
    }

    // [Benchmark]
    public double RatioInt128()
    {
        var ratio = 0d;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            ratio += (double)this.int128Data[i] / (double)this.int128Data[N - 1 - i];
        }

        return ratio;
    }

    // [Benchmark]
    public double RatioInt128B()
    {
        var ratio = 0d;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            ratio += (double)(long)this.int128Data[i] / (double)(long)this.int128Data[N - 1 - i];
        }

        return ratio;
    }

    // [Benchmark]
    public double RatioDecimal()
    {
        var ratio = 0d;
        for (var i = 0; i < this.decimalData.Length; i++)
        {
            ratio += (double)this.decimalData[i] / (double)this.decimalData[N - 1 - i];
        }

        return ratio;
    }

    [Benchmark]
    public long Sum2Long()
    {
        long sum = 0;
        for (var i = 0; i < this.longData.Length; i++)
        {
            sum += (long)((double)this.longData[i] * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)((double)this.int128Data[i] * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128B()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)(this.int128Data[i].ToDouble() * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128C()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)(this.int128Data[i].ToDouble2() * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128X()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)((double)(long)this.int128Data[i] * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128D()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)(this.int128Data[i].ToDouble2() * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128E()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)(long)(this.int128Data[i].ToDouble2() * 0.3d);
        }

        return sum;
    }

    // [Benchmark]
    public Decimal Sum2Decimal()
    {
        Decimal sum = 0;
        for (var i = 0; i < this.decimalData.Length; i++)
        {
            sum += (Decimal)((double)this.decimalData[i] * 0.3d);
        }

        return sum;
    }
}
