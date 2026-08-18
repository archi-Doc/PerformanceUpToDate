// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate;

public enum TestEnum : byte
{
    A,
    B,
    C,
    D = 5,

    Count,
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
    private string?[] enumTable;

    [GlobalSetup]
    public void Setup()
    {
        this.enumTable = CreateEnumTable();
    }

    [Benchmark]
    public string Enum_GetName()
        => Enum.GetName(typeof(TestEnum), this.enumValue);

    [Benchmark]
    public string Enum_GetName2()
        => Enum.GetName(this.enumValue);

    [Benchmark]
    public string? Enum_Table()
        => this.enumTable[(int)this.enumValue];

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

    private static string?[] CreateEnumTable()
    {
        var values = Enum.GetValues<TestEnum>();
        if (values.Length == 0)
        {
            return [];
        }

        var table = new string[(int)TestEnum.Count + 1];
        for (var i = 0; i < values.Length; i++)
        {
            table[(int)values[i]] = Enum.GetName(values[i]);
        }

        return table;
    }
}
