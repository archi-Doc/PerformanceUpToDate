// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1401 // Fields should be private

namespace PerformanceUpToDate
{
    [Config(typeof(BenchmarkConfig))]
    public class MemoryAllocationTest
    {
        public MemoryAllocationTest()
        {
        }

        [Params(10, 100, 256, 512, 1_000, 10_000, 100_000)]
        public int Size { get; set; }

        [Benchmark]
        public void Allocate_New()
        {
            DeadCodeEliminationHelper.KeepAliveWithoutBoxing(new byte[this.Size]);
        }

        [Benchmark]
        public void Allocate_ArrayPool()
        {
            var pool = ArrayPool<byte>.Shared;
            var b = pool.Rent(this.Size);
            pool.Return(b);
        }

        [Benchmark]
        public byte[] AllocateWrite_New()
        {
            var b = new byte[this.Size];
            for (var n = 0; n < this.Size; n++)
            {
                b[n] = 1;
            }

            return b;
        }

        [Benchmark]
        public void AllocateWrite_Stackalloc()
        {
            Span<byte> span = stackalloc byte[this.Size];
            for (var n = 0; n < this.Size; n++)
            {
                span[n] = 1;
            }
        }

        [Benchmark]
        public void AllocateWrite_ArrayPool()
        {
            var pool = ArrayPool<byte>.Shared;
            var b = pool.Rent(this.Size);

            for (var n = 0; n < this.Size; n++)
            {
                b[n] = 1;
            }

            pool.Return(b);
        }
    }
}
