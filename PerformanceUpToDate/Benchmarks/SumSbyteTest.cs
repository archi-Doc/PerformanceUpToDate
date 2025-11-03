// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        sum = this.TestAvx2();
        var sum2 = this.TestAvx2b();
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

    [Benchmark]
    public long TestAvx2b()
    {
        return SumAvx2b(this.data.AsSpan());
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe long SumAvx2b(ReadOnlySpan<sbyte> span)
    {
        long sum = 0;
        int i = 0;

        if (span.Length >= 32)
        {
            var accumulator = Vector256<int>.Zero;

            for (; i <= span.Length - 32; i += 32)
            {
                var bytes = Avx2.LoadVector256((sbyte*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(span.Slice(i))));

                var low16 = Avx2.ConvertToVector256Int16(bytes.GetLower());
                var high16 = Avx2.ConvertToVector256Int16(bytes.GetUpper());

                var low32_1 = Avx2.ConvertToVector256Int32(low16.GetLower());
                var low32_2 = Avx2.ConvertToVector256Int32(low16.GetUpper());
                var high32_1 = Avx2.ConvertToVector256Int32(high16.GetLower());
                var high32_2 = Avx2.ConvertToVector256Int32(high16.GetUpper());

                accumulator = Avx2.Add(accumulator, low32_1);
                accumulator = Avx2.Add(accumulator, low32_2);
                accumulator = Avx2.Add(accumulator, high32_1);
                accumulator = Avx2.Add(accumulator, high32_2);
            }

            var acc128 = Sse2.Add(accumulator.GetLower(), accumulator.GetUpper());
            acc128 = Sse2.Add(acc128, Sse2.Shuffle(acc128, 0b_11_10_11_10));
            acc128 = Sse2.Add(acc128, Sse2.Shuffle(acc128, 0b_01_01_01_01));
            sum = Sse2.ConvertToInt32(acc128);
        }

        for (; i < span.Length; i++)
        {
            sum += span[i];
        }

        return sum;
    }
}
