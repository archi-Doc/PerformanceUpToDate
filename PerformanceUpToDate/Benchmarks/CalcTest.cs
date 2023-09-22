// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using CommandLine.Text;

#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable CS0414
#pragma warning disable CS0649

namespace PerformanceUpToDate;

internal class CalcClass
{
    private int x = 2;
}

internal readonly struct Int128Ripper
{
    public Int128Ripper(Int128 value)
    {
    }

#if BIGENDIAN
    public readonly ulong Upper;
    public readonly ulong Lower;
#else
    public readonly ulong Lower;
    public readonly ulong Upper;
#endif

    public override string ToString()
        => $"({this.Upper}, {this.Lower})";

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
    private const double DoubleToIntThreshold = 1_000_000_000_000_000_000d;

    public static unsafe double ToDouble(this Int128 value)
    {
        /*ulong* x = (ulong*)&value;
        if (x[1] == 0)
        {
            return (double)x[0];
        }
        else if (~x[1] == 0)
        {
            return (double)(long)x[0];
        }
        else
        {
            return (double)value;
        }*/

        var ripper = Unsafe.As<Int128, Int128Ripper>(ref value);
        if (ripper.Upper == 0)
        {
            return (double)ripper.Lower;
        }
        else if (~ripper.Upper == 0)
        {
            return (double)(long)ripper.Lower;
        }
        else
        {
            return (double)value;
        }
    }

    public static unsafe double ToDouble(this UInt128 value)
    {
        var ripper = Unsafe.As<UInt128, Int128Ripper>(ref value);
        if (ripper.Upper == 0)
        {
            return (double)ripper.Lower;
        }
        else
        {
            return (double)value;
        }
    }

    /*public static double ToDouble2(this Int128 value)
    {
        return (value >> 64) == 0 ? (double)(long)value : (double)value;
    }

    public static double ToDouble3(this Int128 value)
    {
        return Int128.LeadingZeroCount(value) >= 64 ? (double)(long)value : (double)value;
    }*/

    public static Int128 ToInt128(this double value)
    {
        if (value >= -DoubleToIntThreshold && value <= +DoubleToIntThreshold)
        {
            return (Int128)(long)value; // new(0, (ulong)Math.Round(value));
        }

        return (Int128)value;
    }

    public static UInt128 ToUInt128(this double value)
    {
        if (value >= 0d && value <= +DoubleToIntThreshold)
        {
            return (UInt128)(long)value;
        }

        return (UInt128)value;
    }
}

[Config(typeof(BenchmarkConfig))]
public class CalcTest
{
    private const int N = 100;
    private long[] longData = new long[N];
    private Int128[] int128Data = new Int128[N];
    private decimal[] decimalData = new decimal[N];
    private double[] doubleData = new double[N];

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
            this.doubleData[i] = i + 1;
        }

        /*var totalLong = this.TotalLong();
        var totalInt128 = this.TotalInt128();
        var totalDecimal = this.TotalDecimal();
        var ratioLong = this.RatioLong();
        var ratioInt128 = this.RatioInt128Naive();
        var ratioDecimal = this.RatioDecimal();*/

        // var f = -1d;
        // var ul = checked((ulong)f);
        var d = ((Int128)(1)).ToDouble();
        d = ((Int128)(0)).ToDouble();
        d = ((Int128)(-1)).ToDouble();
        d = ((Int128)(-10)).ToDouble();
        d = ((Int128)(0.1)).ToDouble();
        d = ((Int128)(-0.1)).ToDouble();
        d = ((Int128)(-11.1)).ToDouble();
        d = ((Int128)(9223372036854775800)).ToDouble();
        d = ((Int128)(-9223372036854775800)).ToDouble();
        d = ((Int128)(19223372036854775800d)).ToDouble();
        d = ((Int128)(-19223372036854775800d)).ToDouble();
        d = ((Int128)(123_000_000_000_000_000_000_000_000d)).ToDouble();
        d = ((Int128)(-123_000_000_000_000_000_000_000_000d)).ToDouble();

        d = ((UInt128)(1)).ToDouble();
        d = ((UInt128)(0)).ToDouble();
        d = ((UInt128)(-1)).ToDouble();
        d = ((UInt128)(-10)).ToDouble();
        d = ((UInt128)(0.1)).ToDouble();
        d = ((UInt128)(-0.1)).ToDouble();
        d = ((UInt128)(-11.1)).ToDouble();
        d = ((UInt128)(9223372036854775800)).ToDouble();
        d = ((UInt128)(-9223372036854775800)).ToDouble();
        d = ((UInt128)(19223372036854775800d)).ToDouble();
        d = ((UInt128)(-19223372036854775800d)).ToDouble();
        d = ((UInt128)(123_000_000_000_000_000_000_000_000d)).ToDouble();
        d = ((UInt128)(-123_000_000_000_000_000_000_000_000d)).ToDouble();

        var x = (0d).ToInt128();
        x = (1d).ToInt128();
        x = (-1d).ToInt128();
        x = (10.1d).ToInt128();
        x = (-10.1d).ToInt128();
        x = (9223372036854775800d).ToInt128();
        x = (-9223372036854775800d).ToInt128();
        x = (19223372036854775800d).ToInt128();
        x = (-19223372036854775800d).ToInt128();
        x = (123_000_000_000_000_000_000_000_000d).ToInt128();
        x = (-123_000_000_000_000_000_000_000_000d).ToInt128();

        var y = (0d).ToUInt128();
        y = (1d).ToUInt128();
        y = (-1d).ToUInt128();
        y = (10.1d).ToUInt128();
        y = (-10.1d).ToUInt128();
        y = (9223372036854775800d).ToUInt128();
        y = (-9223372036854775800d).ToUInt128();
        y = (19223372036854775800d).ToUInt128();
        y = (-19223372036854775800d).ToUInt128();
        y = (123_000_000_000_000_000_000_000_000d).ToUInt128();
        y = (-123_000_000_000_000_000_000_000_000d).ToUInt128();
    }

    // [Benchmark]
    public UInt128 DoubleToUInt128()
    {
        var total = default(UInt128);
        for (var i = 0; i < this.doubleData.Length; i++)
        {
            total += this.doubleData[i].ToUInt128();
        }

        return total;
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
    public double RatioInt128Naive()
    {
        var ratio = 0d;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            ratio += (double)this.int128Data[i] / (double)this.int128Data[N - 1 - i];
        }

        return ratio;
    }

    [Benchmark]
    public double RatioInt128Opt()
    {
        var ratio = 0d;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            ratio += this.int128Data[i].ToDouble() / this.int128Data[N - 1 - i].ToDouble();
        }

        return ratio;
    }

    [Benchmark]
    public double RatioDecimal()
    {
        var ratio = 0d;
        for (var i = 0; i < this.decimalData.Length; i++)
        {
            ratio += (double)(this.decimalData[i] / this.decimalData[N - 1 - i]);
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
    public Int128 Sum2Int128Naive()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)((double)this.int128Data[i] * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128Opt()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (this.int128Data[i].ToDouble() * 0.3d).ToInt128();
        }

        return sum;
    }

    [Benchmark]
    public Int128 Sum2Int128Long()
    {
        Int128 sum = 0;
        for (var i = 0; i < this.int128Data.Length; i++)
        {
            sum += (Int128)(long)((double)(long)this.int128Data[i] * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Decimal Sum2Decimal()
    {
        Decimal sum = 0;
        for (var i = 0; i < this.decimalData.Length; i++)
        {
            sum += (Decimal)((double)this.decimalData[i] * 0.3d);
        }

        return sum;
    }

    [Benchmark]
    public Decimal Sum2Decimal2()
    {
        Decimal sum = 0;
        for (var i = 0; i < this.decimalData.Length; i++)
        {
            sum += this.decimalData[i] * 0.3m;
        }

        return sum;
    }
}
