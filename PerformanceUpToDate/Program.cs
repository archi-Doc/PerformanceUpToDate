// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

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
                typeof(StructTest),
                typeof(DelegateTest),
                typeof(MemoryAllocationTest),
                typeof(ByteCopyTest),
                typeof(ByteCompareTest),
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
            // this.Add(BenchmarkDotNet.Jobs.Job.ShortRun);
            this.Add(BenchmarkDotNet.Jobs.Job.MediumRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class DelegateTest
    {
        private uint count = 0;

        private Func<uint, uint> increaseDelegate = (count) =>
        {
            unchecked
            {
                return count++;
            }
        };

        public DelegateTest()
        {
        }

        [Benchmark]
        public uint Direct()
        {
            unchecked
            {
                return this.count++;
            }
        }

        [Benchmark]
        public uint Method()
        {
            return this.IncreaseMethod();
        }

        [Benchmark]
        public uint Delegate()
        {
            return this.increaseDelegate(this.count);
        }

        private uint IncreaseMethod()
        {
            unchecked
            {
                return this.count++;
            }
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
