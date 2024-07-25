// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public struct MemoryMarshalTestStruct
{
    public MemoryMarshalTestStruct(int x, double y, ulong z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public int X;
    public double Y;
    public ulong Z;
}

[Config(typeof(BenchmarkConfig))]
public class MemoryMarshalTest
{
    private byte[] byteArray = new byte[32];
    private byte[] byteArray2 = new byte[32];
    private MemoryMarshalTestStruct testStruct = new(123, 2345d, 456789123);

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
    public ulong Unsafe_Read()
    {// 0ns
        var span = this.byteArray.AsSpan();
        Unsafe.ReadUnaligned<double>(ref MemoryMarshal.GetReference(span));
        span = span.Slice(8);
        return Unsafe.ReadUnaligned<ulong>(ref MemoryMarshal.GetReference(span));
    }

    [Benchmark]
    public ulong MemoryMarshal_Read()
    {// 0ns
        var span = this.byteArray.AsSpan();
        MemoryMarshal.Read<double>(span);
        span = span.Slice(8);
        return MemoryMarshal.Read<ulong>(span);
    }

    [Benchmark]
    public ulong BitConverter_ToUInt64()
    {// 0ns
        var span = this.byteArray.AsSpan();
        BitConverter.ToDouble(span);
        span = span.Slice(8);
        return BitConverter.ToUInt64(span);
    }

    [Benchmark]
    public byte[] Unsafe_Write()
    {// 0.3ns
        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(this.byteArray2.AsSpan()), this.testStruct);
        return this.byteArray2;
    }

    [Benchmark]
    public byte[] MemoryMarshal_Write()
    {// 0.6ns
        MemoryMarshal.Write(this.byteArray2.AsSpan(), this.testStruct);
        return this.byteArray2;
    }

    /*[Benchmark]
    public byte[] BitConverter_Write()
    {
        BitConverter.TryWriteBytes(this.byteArray2, this.testStruct);
        return this.byteArray2;
    }*/
}
