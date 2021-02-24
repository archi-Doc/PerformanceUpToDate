// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace PerformanceUpToDate.BitTest
{
    [Config(typeof(BenchmarkConfig))]
    public class NLZ
    {
        [Params(10, 1000, 1000_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public int Log2()
        {
            int result = 0;
            var value = N;

            while (value > 0)
            {
                result++;
                value >>= 1;
            }

            var nlz = 32 - result;
            return nlz;
        }

        [Benchmark]
        public int NoBranch()
        {
            int x, y, m, n;

            x = N;
            y = -(x >> 16);
            m = (y >> 16) & 16;
            n = 16 - m;
            x = x >> m;

            y = x - 0x100;
            m = (y >> 16) & 8;
            n = n + m;
            x = x << m;

            y = x - 0x1000;
            m = (y >> 16) & 4;
            n = n + m;
            x = x << m;

            y = x - 0x4000;
            m = (y >> 16) & 2;
            n = n + m;
            x = x << m;

            y = x >> 14;
            m = y & ~(y >> 1);

            var nlz = n + 2 - m;
            return nlz;
        }

        [Benchmark]
        public int Branch()
        {
            int x, y;
            int n, nlz;

            x = N;
            n = 32;

            y = x >> 16;
            if (y != 0)
            {
                n = n - 16;
                x = y;
            }

            y = x >> 8;
            if (y != 0)
            {
                n = n - 8;
                x = y;
            }

            y = x >> 4;
            if (y != 0)
            {
                n = n - 4;
                x = y;
            }

            y = x >> 2;
            if (y != 0)
            {
                n = n - 2;
                x = y;
            }

            y = x >> 1;
            if (y != 0)
            {
                nlz = n - 2;
            }
            else
            {
                nlz = n - x;
            }

            return nlz;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct LongDoubleUnion
        {
            [FieldOffset(0)]
            internal long Long;

            [FieldOffset(0)]
            internal double Double;
        }

        [Benchmark]
        public int Union()
        {
            var union = default(LongDoubleUnion);
            union.Double = (double)N + 0.5;
            var nlz = (int)(1054 - (union.Long >> 52));
            return nlz;
        }
    }
}
