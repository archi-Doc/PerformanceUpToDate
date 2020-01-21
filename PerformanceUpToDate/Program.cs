﻿// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

/*  BenchmarkDotNet, small template code
 *  PM> Install-Package BenchmarkDotNet
 */

using System;
using System.Buffers;
using System.Linq;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;

namespace PerformanceUpToDate
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // var summary = BenchmarkRunner.Run<ByteCopyTest>(); // SwapTest, MemoryAllocationTest, ByteCopyTest
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(SwapTest),
                typeof(MemoryAllocationTest),
                typeof(ByteCopyTest),
            });
            switcher.Run(args);
        }
    }

    public class BenchmarkConfig : BenchmarkDotNet.Configs.ManualConfig
    {
        public BenchmarkConfig()
        {
            this.Add(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
            this.Add(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);

            // this.Add(Job.ShortRun.With(BenchmarkDotNet.Environments.Platform.X64).WithWarmupCount(1).WithIterationCount(1));
            // this.Add(BenchmarkDotNet.Jobs.Job.MediumRun.WithGcForce(true).WithId("GcForce medium"));
            this.Add(BenchmarkDotNet.Jobs.Job.ShortRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class SwapTest
    {
        private int x = 0;
        private int y = 1;
        private int z = 0;
        private SwapClass swap;

        public SwapTest()
        {
            this.swap = new SwapClass();
        }

        [Benchmark]
        public SwapClass Swap_Temp()
        {
            int temp;

            temp = this.swap.a;
            this.swap.a = this.swap.b;
            this.swap.b = temp;

            return this.swap;
        }

        [Benchmark]
        public SwapClass Swap_Tuple()
        {
            (this.swap.a, this.swap.b) = (this.swap.b, this.swap.a);

            return this.swap;
        }

        public class SwapClass
        {
            public int a = 0;
            public int b = 1;
            public int c = 2;
        }
    }

    // [Orderer(SummaryOrderPolicy.Default,  MethodOrderPolicy.Alphabetical)]
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
