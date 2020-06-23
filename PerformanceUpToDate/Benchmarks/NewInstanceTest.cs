// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate.NewInstance
{
    public enum TestEnum
    {
        One,
        Two,
    }

    public struct TestStruct
    {
    }

    public struct TestStruct2
    {
        public int X;
        public int Y;
    }

    public class TestClass
    {
    }

    public class TestClass2
    {
#pragma warning disable SA1401 // Fields should be private
        public int X;
        public int Y;
#pragma warning restore SA1401 // Fields should be private
    }

    public class TestClass3
    {
        public int X { get; set; }

        public int Y { get; set; }
    }

    [Config(typeof(BenchmarkConfig))]
    public class NewInstanceTest
    {
        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public int NewInt()
        {
            return 1;
        }

        [Benchmark]
        public TestEnum NewEnum()
        {
            return default(TestEnum);
        }

        [Benchmark]
        public TestStruct NewStruct()
        {
            return default(TestStruct);
        }

        [Benchmark]
        public TestStruct2 NewStruct2()
        {
            return default(TestStruct2);
        }

        [Benchmark]
        public TestClass NewClass()
        {
            return new TestClass();
        }

        [Benchmark]
        public TestClass2 NewClass2()
        {
            return new TestClass2();
        }

        [Benchmark]
        public TestClass3 NewClass3()
        {
            return new TestClass3();
        }
    }
}
