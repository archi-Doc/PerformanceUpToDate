// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate;

public enum TestEnum : byte
{
    A,
    B,
    C,
    D,
}

[Config(typeof(BenchmarkConfig))]
public class EnumTest
{
    public EnumTest()
    {
        this.enumValue = TestEnum.C;
        this.byteValue = (byte)2;
    }

    private TestEnum enumValue;
    private byte byteValue;

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public byte EnumToByte_Cast()
        => (byte)this.enumValue;

    [Benchmark]
    public byte EnumToByte_Unsafe()
    {
        return Unsafe.As<TestEnum, byte>(ref this.enumValue);
    }

    [Benchmark]
    public TestEnum ByteToEnum_Cast()
        => (TestEnum)this.byteValue;

    [Benchmark]
    public TestEnum ByteToEnum_Unsafe()
    {
        return Unsafe.As<byte, TestEnum>(ref this.byteValue);
    }

    [Benchmark]
    public TestEnum ByteToEnum_ToObject()
        => (TestEnum)Enum.ToObject(typeof(TestEnum), this.byteValue);
}
