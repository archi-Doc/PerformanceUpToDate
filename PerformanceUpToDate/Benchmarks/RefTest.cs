// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1401 // Fields should be private

namespace PerformanceUpToDate
{
    [Config(typeof(BenchmarkConfig))]
    public class RefTest
    {
        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public int IncreaseValue()
        {
            int value = 0;
            for (var n = 0; n < 1000; n++)
            {
                value = this.IncreaseValueCore(value);
            }

            return value;
        }

        [Benchmark]
        public int IncreaseRef()
        {
            int value = 0;
            for (var n = 0; n < 1000; n++)
            {
                this.IncreaseRefCore(ref value);
            }

            return value;
        }

        private int IncreaseValueCore(int v) => v++;

        private void IncreaseRefCore(ref int v) => v++;
    }
}
