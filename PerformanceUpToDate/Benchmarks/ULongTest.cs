// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[StructLayout(LayoutKind.Explicit)]
public struct ULongStruct
{
    [FieldOffset(0)]
    public byte Byte;

    [FieldOffset(1)]
    public short Short;

    [FieldOffset(3)]
    public sbyte SByte;

    [FieldOffset(4)]
    public uint UInt;

    public ulong ULong
        => Unsafe.As<ULongStruct, ulong>(ref this);
}

[Config(typeof(BenchmarkConfig))]
public class ULongTest
{
    private ulong u;
    private ULongStruct st;

    public ULongTest()
    {
        for (var i = 0; i < 300; i++)
        {
            this.IncrementSByte_Struct();
            this.IncrementSByte_ULong();
        }

        Debug.Assert(this.u == this.st.ULong);
    }

    [Benchmark]
    public sbyte IncrementSByte_Struct()
    {
        return this.st.SByte++;
    }

    [Benchmark]
    public sbyte IncrementSByte_ULong()
    {
        const ulong Mask = 0xFF00_0000;
        var s = (sbyte)((this.u & Mask) >> 24);
        s++;
        this.u = (this.u & ~Mask) | (((ulong)s << 24) & Mask);
        return s;
    }
}
