// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1401 // Fields should be private

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class SpanTest
{
    public byte[] byteArray = default!;
    public byte[] byteArray2 = default!;
    public ReadOnlyMemory<byte> memory;

    [GlobalSetup]
    public void Setup()
    {
        this.byteArray = new byte[1000];
        this.byteArray2 = new byte[1000];
        this.memory = this.byteArray.AsMemory();
    }

    [Benchmark]
    public ReadOnlySpan<byte> Span()
    {
        return this.byteArray.AsSpan();
    }

    [Benchmark]
    public ReadOnlySpan<byte> MemoryToSpan()
    {
        return this.memory.Span;
    }

    [Benchmark]
    public ReadOnlyMemory<byte> SpanToMemory()
    {
        return this.byteArray.AsSpan().ToArray().AsMemory();
    }

    /* [Benchmark]
    public Memory<byte> SpanTest_Memory()
    {
        return this.byteArray.AsMemory().Slice(10, 30);
    }

    [Benchmark]
    public Memory<byte> SpanTest_Memory2()
    {
        return ((Memory<byte>)this.byteArray).Slice(10, 30);
    }

    [Benchmark]
    public Memory<byte> SpanTest_Memory3()
    {
        var m = (Memory<byte>)this.byteArray;
        return m.Slice(10, 30);
    }*/

    /*[Benchmark]
    public byte SpanTest_Copy()
    {
        var sp = this.byteArray.AsSpan();
        var sp2 = this.byteArray2.AsSpan();

        sp.CopyTo(sp2);

        return sp2[10];
    }

    [Benchmark]
    public byte SpanTest_Copy2()
    {
        var sp = this.byteArray.AsSpan();
        MemoryMarshal.TryGetArray<byte>(this.byteArray2, out var seg);

        seg.AsSpan().CopyTo(sp);

        return sp[10];
    }*/

    /*[Benchmark]
    public byte SpanTest_Slice()
    {
        var sp = new byte[100].AsSpan();
        for (var n = 0; n < 90; n++)
        {
            sp = sp.Slice(1);
            var sp2 = sp;
        }

        return sp[0];
    }

    [Benchmark]
    public byte SpanTest_Slice2()
    {
        var sp = new byte[100].AsSpan();
        var position = 0;
        for (var n = 0; n < 90; n++)
        {
            position++;
            var sp2 = sp.Slice(position, 1);
        }

        return sp[0];
    }*/
}
