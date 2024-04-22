// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class MemoryMarshalTest
{
    private byte[] byteArray = new byte[32];

    public MemoryMarshalTest()
    {
        this.byteArray[0] = 12;
        this.byteArray[1] = 34;
        this.byteArray[2] = 56;
        this.byteArray[3] = 78;

        var a = this.MemoryMarshal_Read();
        var b = this.BitConverter_ToUInt64();
    }

    [Benchmark]
    public ulong MemoryMarshal_Read()
    {
        var span = this.byteArray.AsSpan();
        MemoryMarshal.Read<double>(span);
        span = span.Slice(8);
        return MemoryMarshal.Read<ulong>(span);
    }

    [Benchmark]
    public ulong BitConverter_ToUInt64()
    {
        var span = this.byteArray.AsSpan();
        BitConverter.ToDouble(span);
        span = span.Slice(8);
        return BitConverter.ToUInt64(span);
    }
}
