// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CountLeadingSpacesTest
{
    // private readonly string text = "            123456";
    private readonly string text = "                  123456";

    public CountLeadingSpacesTest()
    {
    }

    [Benchmark]
    public unsafe int CountLeadingSpaces_Unsafe()
    {
        var span = this.text.AsSpan();
        var i = 0;
        var remaining = span.Length;
        fixed (char* c = span)
        {
            ulong* p = (ulong*)c;
            while (remaining >= 4)
            {
                if (*p == 0x0020002000200020)
                {
                    p++;
                    i += 4;
                    remaining -= 4;
                }
                else
                {
                    break;
                }
            }

            char* d = (char*)p;
            while (remaining-- > 0)
            {
                if (*d == ' ')
                {
                    d++;
                    i++;
                }
                else
                {
                    return i;
                }
            }

            return i;
        }
    }

    [Benchmark]
    public int CountLeadingSpaces_Vector()
    {
        var span = this.text.AsSpan();
        var i = 0;

        if (Vector.IsHardwareAccelerated && span.Length >= Vector<ushort>.Count)
        {
            var space = new Vector<ushort>(' ');

            var ushortSpan = System.Runtime.InteropServices.MemoryMarshal.Cast<char, ushort>(span);
            int vectorCount = Vector<ushort>.Count;
            int last = ushortSpan.Length - vectorCount;

            while (i <= last)
            {
                var v = new Vector<ushort>(ushortSpan.Slice(i, vectorCount));
                var eq = Vector.Equals(v, space);

                if (Vector.EqualsAll(eq, Vector<ushort>.AllBitsSet))
                {
                    i += vectorCount;
                    continue;
                }

                for (int j = 0; j < vectorCount; j++)
                {
                    if (ushortSpan[i + j] != ' ')
                        return i + j;
                }
            }
        }

        while ((uint)i < (uint)span.Length && span[i] == ' ')
        {
            i++;
        }

        return i;
    }

    [Benchmark]
    public int CountLeadingSpaces_Scalar()
    {
        var span = this.text.AsSpan();
        var i = 0;
        while ((uint)i < (uint)span.Length && span[i] == ' ')
        {
            i++;
        }

        return i;
    }

    [Benchmark]
    public int CountLeadingSpaces_Scalar2()
    {
        var span = this.text.AsSpan();
        var i = 0;
        while (i < span.Length && span[i] == ' ')
        {
            i++;
        }

        return i;
    }

    [Benchmark]
    public int CountLeadingSpaces_SSE2()
    {
        var span = this.text.AsSpan();
        if (Avx2.IsSupported)
        {
            return CountAvx2(span);
        }
        else if (Sse2.IsSupported)
        {
            return CountSse2(span);
        }
        else
        {
            return CountScalar(span);
        }
    }

    [Benchmark]
    public unsafe int CountLeadingSpaces()
    {
        var span = this.text.AsSpan();
        var length = span.Length;
        if (length == 0 || span[0] != ' ')
        {
            return 0;
        }

        var i = 0;
        if (length < 8)
        {
            i = 1;
            while (i < length && span[i] == ' ')
            {
                i++;
            }

            return i;
        }

        fixed (char* p = span)
        {
            if (Avx2.IsSupported)
            {
                Vector256<ushort> spaces = Vector256.Create((ushort)' ');
                while (i <= length - 16)
                {
                    Vector256<ushort> chunk = Avx.LoadVector256((ushort*)(p + i));
                    Vector256<ushort> eq = Avx2.CompareEqual(chunk, spaces);
                    uint nonSpaceMask = ~(uint)Avx2.MoveMask(eq.AsByte()) & 0x55555555u;
                    if (nonSpaceMask != 0)
                    {
                        return i + (BitOperations.TrailingZeroCount(nonSpaceMask) >> 1);
                    }

                    i += 16;
                }
            }

            if (Sse2.IsSupported)
            {
                Vector128<ushort> spaces = Vector128.Create((ushort)' ');

                while (i <= length - 8)
                {
                    Vector128<ushort> chunk = Sse2.LoadVector128((ushort*)(p + i));
                    Vector128<ushort> eq = Sse2.CompareEqual(chunk, spaces);
                    uint nonSpaceMask = ~(uint)Sse2.MoveMask(eq.AsByte()) & 0x5555u;
                    if (nonSpaceMask != 0)
                    {
                        return i + (BitOperations.TrailingZeroCount(nonSpaceMask) >> 1);
                    }

                    i += 8;
                }
            }

            while (i < length && p[i] == ' ')
            {
                i++;
            }
        }

        return i;
    }

    private static unsafe int CountAvx2(ReadOnlySpan<char> span)
    {
        var vSpace = Vector256.Create((ushort)' ');
        var i = 0;
        fixed (char* p = span)
        {
            while (i + 16 <= span.Length)
            {
                var chunk = Avx.LoadVector256((ushort*)(p + i));
                var eq = Avx2.CompareEqual(chunk, vSpace);
                int mask = ~Avx2.MoveMask(eq.AsByte());

                if (mask != 0)
                {
                    return i + (BitOperations.TrailingZeroCount((uint)mask) / 2);
                }

                i += 16;
            }

            if (Sse2.IsSupported && i + 8 <= span.Length)
            {
                var vS128 = Vector128.Create((ushort)' ');
                var chunk = Sse2.LoadVector128((ushort*)(p + i));
                var eq = Sse2.CompareEqual(chunk, vS128);
                int mask = ~Sse2.MoveMask(eq.AsByte());

                if (mask != 0)
                {
                    return i + (BitOperations.TrailingZeroCount((uint)mask) / 2);
                }

                i += 8;
            }
        }

        return i + CountScalar(span.Slice(i));
    }

    private static unsafe int CountSse2(ReadOnlySpan<char> span)
    {
        var vSpace = Vector128.Create((ushort)' ');
        var i = 0;
        fixed (char* p = span)
        {
            while (i + 8 <= span.Length)
            {
                var chunk = Sse2.LoadVector128((ushort*)(p + i));
                var eq = Sse2.CompareEqual(chunk, vSpace);
                var mask = ~Sse2.MoveMask(eq.AsByte());

                if (mask != 0)
                {
                    return i + (BitOperations.TrailingZeroCount((uint)mask) / 2);
                }

                i += 8;
            }
        }

        return i + CountScalar(span.Slice(i));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int CountScalar(ReadOnlySpan<char> span)
    {
        var i = 0;
        while (i < span.Length && span[i] == ' ')
        {
            i++;
        }

        return i;
    }
}
