// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class SumSbyteTest
{
    // private readonly sbyte[] data = [0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0,];

    private readonly sbyte[] data = [0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1, -1, 1, 0, 0, 1,];

    public SumSbyteTest()
    {
        var length = this.data.Length;
        var sum = this.Test1();
        var sum2 = this.TestAvx2();
    }

    [Benchmark]
    public int Test1()
    {
        var span = this.data.AsSpan();
        int sum = 0;
        for (int i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }

        return sum;
    }

    [Benchmark]
    public int TestAvx2()
    {
        return SumAvx2(this.data.AsSpan());
    }

    static unsafe int SumAvx2(ReadOnlySpan<sbyte> span)
    {
        var length = span.Length;
        var i = 0;
        var sumVec = Vector256<sbyte>.Zero;
        fixed (sbyte* ptr = span)
        {
            for (; i + 32 <= length; i += 32)
            {
                var v = Avx.LoadVector256(ptr + i);
                sumVec = Avx2.Add(sumVec, v);
            }
        }

        Span<sbyte> tmp = stackalloc sbyte[32];
        fixed (sbyte* tmpPtr = tmp)
        {
            Avx.Store(tmpPtr, sumVec);
        }

        var sum = 0;
        for (int j = 0; j < 32; j++)
        {
            sum += tmp[j];
        }

        for (; i < length; i++)
        {
            sum += span[i];
        }

        return sum;
    }
}
