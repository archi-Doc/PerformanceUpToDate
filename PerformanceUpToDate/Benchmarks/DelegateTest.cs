// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1401 // Fields should be private

namespace PerformanceUpToDate;

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
