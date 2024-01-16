// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Numerics;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CubicRootTest
{
    private static ReadOnlySpan<byte> v => new byte[]
    {
        0, 54, 54, 54,  118, 118, 118, 118,
        123,  129,  134,  138,  143,  147,  151,  156,
        157,  161,  164,  168,  170,  173,  176,  179,
        181,  185,  187,  190,  192,  194,  197,  199,
        200,  202,  204,  206,  209,  211,  213,  215,
        217,  219,  221,  222,  224,  225,  227,  229,
        231,  232,  234,  236,  237,  239,  240,  242,
        244,  245,  246,  248,  250,  251,  252,  254,
    };

    private static ReadOnlySpan<uint> v2 => new uint[]
    {
        0, 54, 54, 54,  118, 118, 118, 118,
        123,  129,  134,  138,  143,  147,  151,  156,
        157,  161,  164,  168,  170,  173,  176,  179,
        181,  185,  187,  190,  192,  194,  197,  199,
        200,  202,  204,  206,  209,  211,  213,  215,
        217,  219,  221,  222,  224,  225,  227,  229,
        231,  232,  234,  236,  237,  239,  240,  242,
        244,  245,  246,  248,  250,  251,  252,  254,
    };

    public static uint CubicRoot(ulong a)
    {
        var b = 64 - BitOperations.LeadingZeroCount(a);
        if (b < 7)
        {
            return ((uint)v[(int)a] + 35) >> 6;
        }

        b = ((b * 84) >> 8) - 1;
        var shift = (int)(a >> (b * 3));
        var x = (((uint)v[shift] + 10) << b) >> 6;
        x = (2 * x) + (uint)(a / ((ulong)x * (ulong)(x - 1)));
        x = (x * 341) >> 10;
        return x;
    }

    public static uint CubicRoot2(ulong a)
    {
        var b = 64 - BitOperations.LeadingZeroCount(a);
        if (b < 7)
        {
            return (v2[(int)a] + 35) >> 6;
        }

        b = ((b * 84) >> 8) - 1;
        var shift = (int)(a >> (b * 3));
        var x = ((v2[shift] + 10) << b) >> 6;
        x = (2 * x) + (uint)(a / ((ulong)x * (ulong)(x - 1)));
        x = (x * 341) >> 10;
        return x;
    }

    public static uint ReferenceRoot(ulong a)
        => (uint)Math.Cbrt(a);

    private uint a1 = 10;
    private uint a2 = 100;
    private uint a3 = 1_000;
    private uint a4 = 10_000;
    private uint a5 = 100_000;

    private double d1 = 10;
    private double d2 = 100;

    public CubicRootTest()
    {
    }

    [Benchmark]
    public uint Reference()
    {
        return ReferenceRoot(this.a1) + ReferenceRoot(this.a2) + ReferenceRoot(this.a3) + ReferenceRoot(this.a4) + ReferenceRoot(this.a5);
    }

    [Benchmark]
    public uint NewtonRaphson()
    {
        return CubicRoot(this.a1) + CubicRoot(this.a2) + CubicRoot(this.a3) + CubicRoot(this.a4) + CubicRoot(this.a5);
    }

    [Benchmark]
    public uint NewtonRaphson2()
    {
        return CubicRoot2(this.a1) + CubicRoot2(this.a2) + CubicRoot2(this.a3) + CubicRoot2(this.a4) + CubicRoot2(this.a5);
    }

    [Benchmark]
    public uint MultiplyUint()
        => this.a1 * this.a2;

    [Benchmark]
    public double MultiplyDouble()
        => this.d1 * this.d2;
}
