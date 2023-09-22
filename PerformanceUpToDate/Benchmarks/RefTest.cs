// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

#pragma warning disable SA1000
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter

namespace PerformanceUpToDate.RefTest;

[Config(typeof(BenchmarkConfig))]
public class RefTest1
{
    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public int IncreaseValue()
    {
        int value = 0;
        for (var n = 0; n < 1000; n++)
        {
            value = this.IncreaseValueCore(value);
        }

        return value;
    }

    [Benchmark]
    public int IncreaseRef()
    {
        int value = 0;
        for (var n = 0; n < 1000; n++)
        {
            this.IncreaseRefCore(ref value);
        }

        return value;
    }

    private int IncreaseValueCore(int v) => v++;

    private void IncreaseRefCore(ref int v) => v++;
}

public struct TestStruct
{
    int i;
    string s;
    double d;
    float f;
    byte b;
    long l;
    ushort u;
}

public class TestClass
{
    int i;
    string s;
    double d;
    float f;
    byte b;
    long l;
    ushort u;
}

public class ReuseObject<T>
{
    public readonly T Value;

    public ReuseObject(ref T t)
    {
        this.Value = t;
    }

    public ReuseObject(T t)
    {
        this.Value = t;
    }
}

[Config(typeof(BenchmarkConfig))]
public class RefTest2
{
    public TestClass tc;
    public TestStruct ts;

    [GlobalSetup]
    public void Setup()
    {
        this.tc = new();
        this.ts = default;
    }

    object? ObjectToObject(object? reuse)
    {
        return reuse;
    }

    ref TestStruct RefToRef_Struct(ref TestStruct reuse)
    {
        return ref reuse;
    }

    ref TestClass RefToRef_Class(ref TestClass reuse)
    {
        return ref reuse;
    }

    ReuseObject<T>? ReuseToReuse<T>(ReuseObject<T>? reuse)
    {
        return reuse;
    }

    [Benchmark]
    public object? ObjectToObject_Class()
    {
        return this.ObjectToObject(this.tc);
    }

    [Benchmark]
    public object? ObjectToObject_Struct()
    {
        return this.ObjectToObject(this.ts);
    }

    [Benchmark]
    public TestClass RefToRef_Class()
    {
        return this.RefToRef_Class(ref this.tc);
    }

    [Benchmark]
    public TestStruct RefToRef_Struct()
    {
        return this.RefToRef_Struct(ref this.ts);
    }

    [Benchmark]
    public ReuseObject<TestStruct>? ReuseToReuse_Struct()
    {
        return this.ReuseToReuse<TestStruct>(null);
    }

    [Benchmark]
    public ReuseObject<TestStruct>? ReuseToReuse_Struct2()
    {
        return this.ReuseToReuse<TestStruct>(new ReuseObject<TestStruct>(this.ts));
    }

    [Benchmark]
    public ReuseObject<TestStruct>? ReuseToReuse_Struct3()
    {
        return this.ReuseToReuse<TestStruct>(new ReuseObject<TestStruct>(ref this.ts));
    }
}

[Config(typeof(BenchmarkConfig))]
public class RefTest3
{
    public TestClass? tc;
    public TestClass? tc2;
    public TestStruct ts;
    public object? tss;
    public object? tss2;

    [GlobalSetup]
    public void Setup()
    {
        this.tc = new();
        this.tc2 = null;
        this.ts = default;
        this.tss = this.ts;
        this.tss2 = null;
    }

    [Benchmark]
    public bool Test_NonNullClass_Is()
    {
        if (this.tc is TestClass cls)
        {
            return true;
        }

        return false;
    }

    [Benchmark]
    public bool Test_NullClass_Is()
    {
        if (this.tc2 is TestClass cls)
        {
            return true;
        }

        return false;
    }

    [Benchmark]
    public bool Test_NonNulStruct_Is()
    {
        if (this.tss is TestStruct cls)
        {
            return true;
        }

        return false;
    }

    [Benchmark]
    public bool Test_NullStruct_Is()
    {
        if (this.tss2 is TestStruct cls)
        {
            return true;
        }

        return false;
    }

    [Benchmark]
    public bool Test_NonNullClass_Is2()
    {
        if (this.tc != null && this.tc is TestClass cls)
        {
            return true;
        }

        return false;
    }

    [Benchmark]
    public bool Test_NullClass_Is2()
    {
        if (this.tc2 != null && this.tc2 is TestClass cls)
        {
            return true;
        }

        return false;
    }

    [Benchmark]
    public bool Test_NonNulStruct_Is2()
    {
        if (this.tss != null && this.tss is TestStruct cls)
        {
            return true;
        }

        return false;
    }

    [Benchmark]
    public bool Test_NullStruct_Is2()
    {
        if (this.tss2 != null && this.tss2 is TestStruct cls)
        {
            return true;
        }

        return false;
    }
}
