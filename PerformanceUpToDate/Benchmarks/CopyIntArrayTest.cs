// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate
{
    [Config(typeof(BenchmarkConfig))]
    public class CopyIntArrayTest
    {
        public int[] SourceArray = default!;
        public int[] DestinationArray = default!;

        public CopyIntArrayTest()
        {
        }

        [Params(10, 100)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.SourceArray = new int[this.Size];
            this.DestinationArray = new int[this.Size];
        }

        [Benchmark]
        public int[] ForLoop()
        {
            for (var n = 0; n < this.SourceArray.Length; n++)
            {
                this.DestinationArray[n] = this.SourceArray[n];
            }

            return this.DestinationArray;
        }

        [Benchmark]
        public int[] ArrayCopy()
        {
            Array.Copy(this.SourceArray, this.DestinationArray, this.SourceArray.Length);

            return this.DestinationArray;
        }
    }
}
