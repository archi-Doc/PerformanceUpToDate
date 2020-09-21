// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1401 // Fields should be private

namespace PerformanceUpToDate
{
    [Config(typeof(BenchmarkConfig))]
    public class ImmutableTest
    {
        [Params(10, 100, 1_000)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public int[] ArrayTest()
        {
            var array = new int[this.Size];

            for (var n = 0; n < this.Size; n++)
            {
                array[n] = n;
            }

            return array;
        }

        [Benchmark]
        public ImmutableArray<int> ImmutableArrayTest()
        {
            var builder = ImmutableArray.CreateBuilder<int>();

            for (var n = 0; n < this.Size; n++)
            {
                builder.Add(n);
            }

            return builder.ToImmutable();
        }
    }
}
