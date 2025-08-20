// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CompoundAssignmentTest
{
    private Lock lockObject = new();
    private uint x;
    private uint y;
    private uint z;

    public CompoundAssignmentTest()
    {
        x = 0xcd9a4373;
        y = 0x98388f9b;
        z = 0x3ea32310;
    }

    [Benchmark]
    public uint Raw()
    {
        var r = x | y;
        r |= z;
        return r;
    }

    [Benchmark]
    public uint Lock()
    {
        using (this.lockObject.EnterScope())
        {
            var r = x | y;
            r |= z;
            return r;
        }
    }

    [Benchmark]
    public uint Atomic()
    {
        var r = x;
        AtomicOr(ref r, this.y);
        AtomicOr(ref r, this.z);
        return r;

        static void AtomicOr(ref uint location, uint value)
        {
            uint initialValue, newValue;
            do
            {
                initialValue = Volatile.Read(ref location);
                newValue = initialValue | value;
            }
            while (Interlocked.CompareExchange(ref location, newValue, initialValue) != initialValue);
        }
    }
}
