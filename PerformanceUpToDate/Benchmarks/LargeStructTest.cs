// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Arc.Collections;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public class LargeClass_Struct
{
    public LargeClass_Struct()
    {
        this.LargeReadonlyStruct = new LargeReadonlyStruct(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
    }

    public LargeReadonlyStruct LargeReadonlyStruct { get; private set; }

    public ulong Sum()
        => this.LargeReadonlyStruct.Sum();
}

public class LargeClass2_Struct
{
    public LargeClass2_Struct()
    {
        this.st = new LargeReadonlyStruct();
    }

    private readonly LargeReadonlyStruct st;

    public ulong Sum()
        => this.st.Sum();
}

public class LargeClass_Class2
{
    public LargeClass_Class2()
    {

    }

    public LargeClass Class { get; } = new LargeClass();
}

public class LargeClass_Class3
{
    private static ObjectPool<LargeClass> pool = new(() => new LargeClass(), 1000);

    public LargeClass_Class3()
    {
        this.Class = pool.Get();
    }

    public LargeClass Class { get; }

    public void Return()
    {
        pool.Return(this.Class);
    }
}

public class LargeClass_Class
{
    public LargeClass_Class()
    {
        this.X0 = 0;
        this.X1 = 1;
        this.X2 = 2;
        this.X3 = 3;
        this.X4 = 4;
        this.X5 = 5;
        this.X6 = 6;
        this.X7 = 7;
        this.X8 = 8;
        this.X9 = 9;
    }

    public readonly ulong X0;
    public readonly ulong X1;
    public readonly ulong X2;
    public readonly ulong X3;
    public readonly ulong X4;
    public readonly ulong X5;
    public readonly ulong X6;
    public readonly ulong X7;
    public readonly ulong X8;
    public readonly ulong X9;

    public ulong Sum()
        => this.X0 + this.X1 + this.X2 + this.X3 + this.X4 + this.X5 + this.X6 + this.X7 + this.X8 + this.X9;
}

public class LargeClass
{
    public LargeClass()
    {
        this.X0 = 0;
        this.X1 = 1;
        this.X2 = 2;
        this.X3 = 3;
        this.X4 = 4;
        this.X5 = 5;
        this.X6 = 6;
        this.X7 = 7;
        this.X8 = 8;
        this.X9 = 9;
    }

    public LargeClass(ulong x0, ulong x1, ulong x2, ulong x3, ulong x4, ulong x5, ulong x6, ulong x7, ulong x8, ulong x9)
    {
        this.X0 = x0;
        this.X1 = x1;
        this.X2 = x2;
        this.X3 = x3;
        this.X4 = x4;
        this.X5 = x5;
        this.X6 = x6;
        this.X7 = x7;
        this.X8 = x8;
        this.X9 = x9;
    }

    public readonly ulong X0;
    public readonly ulong X1;
    public readonly ulong X2;
    public readonly ulong X3;
    public readonly ulong X4;
    public readonly ulong X5;
    public readonly ulong X6;
    public readonly ulong X7;
    public readonly ulong X8;
    public readonly ulong X9;

    public ulong Sum()
        => this.X0 + this.X1 + this.X2 + this.X3 + this.X4 + this.X5 + this.X6 + this.X7 + this.X8 + this.X9;
}

public readonly struct LargeReadonlyStruct
{
    public LargeReadonlyStruct(ulong x0, ulong x1, ulong x2, ulong x3, ulong x4, ulong x5, ulong x6, ulong x7, ulong x8, ulong x9)
    {
        this.X0 = x0;
        this.X1 = x1;
        this.X2 = x2;
        this.X3 = x3;
        this.X4 = x4;
        this.X5 = x5;
        this.X6 = x6;
        this.X7 = x7;
        this.X8 = x8;
        this.X9 = x9;
    }

    public readonly ulong X0;
    public readonly ulong X1;
    public readonly ulong X2;
    public readonly ulong X3;
    public readonly ulong X4;
    public readonly ulong X5;
    public readonly ulong X6;
    public readonly ulong X7;
    public readonly ulong X8;
    public readonly ulong X9;

    public ulong Sum()
        => this.X0 + this.X1 + this.X2 + this.X3 + this.X4 + this.X5 + this.X6 + this.X7 + this.X8 + this.X9;
}

[Config(typeof(BenchmarkConfig))]
public class LargeStructTest
{
    public LargeStructTest()
    {
    }

    /*[Benchmark]
    public ulong LargeStruct_Sum()
    {
        var c = new LargeReadonlyStruct();
        return c.Sum();
    }

    [Benchmark]
    public ulong LargeStruct_Sum2()
    {
        var c = new LargeReadonlyStruct(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        return this.Sum(c);
    }

    [Benchmark]
    public ulong LargeStruct_Sum3()
    {
        var c = new LargeReadonlyStruct(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
        return this.Sum(ref c);
    }*/

    [Benchmark]
    public ulong LargeClassStruct_Sum()
    {
        var c = new LargeClass_Struct();
        return c.LargeReadonlyStruct.Sum(); // = c.Sum();
    }

    [Benchmark]
    public ulong LargeClassStruct_Sum2()
    {
        var c = new LargeClass_Struct();
        return 7;
    }

    [Benchmark]
    public ulong LargeClassStruct_Sum3()
    {
        var c = new LargeClass2_Struct();
        return c.Sum();
    }

    [Benchmark]
    public ulong LargeClassClass_Sum()
    {
        var c = new LargeClass_Class();
        return c.Sum();
    }

    [Benchmark]
    public ulong LargeClassClass2_Sum()
    {
        var c = new LargeClass_Class2();
        return c.Class.Sum();
    }

    [Benchmark]
    public ulong LargeClassClass3_Sum()
    {
        var c = new LargeClass_Class3();
        var x = c.Class.Sum();
        c.Return();
        return x;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private ulong Sum(LargeReadonlyStruct c)
        => c.Sum();

    [MethodImpl(MethodImplOptions.NoInlining)]
    private ulong Sum(ref LargeReadonlyStruct c)
        => c.Sum();
}
