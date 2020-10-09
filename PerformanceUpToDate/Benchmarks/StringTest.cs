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
    public class StringTest
    {
        public string TestString = default!;
        public string AppendString = default!;
        public char AppendChar;

        [GlobalSetup]
        public void Setup()
        {
            this.TestString = "test";
            this.AppendString = "1";
            this.AppendChar = '1';
        }

        [Benchmark]
        public string Append()
        {
            return this.TestString + this.AppendString;
        }

        [Benchmark]
        public string Append2()
        {
            return this.TestString + this.AppendChar;
        }
    }
}
