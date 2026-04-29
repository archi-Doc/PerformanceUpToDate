// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public record class NullableTestClass(int X, int Y) : INullableTestInterface
{
    public int Sum() => this.X + this.Y;
}

public record class NullableTestPair(NullableTestClass TestClass, int Z) : INullableTestInterface
{
    public int Sum() => this.TestClass.Sum();
}

public interface INullableTestInterface
{
    int Sum();
}

public readonly record struct NullableTestStruct : INullableTestInterface
{
    public readonly int X;
    public readonly int Y;

    public NullableTestStruct()
    {
    }

    public NullableTestStruct(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public int Sum() => this.X + this.Y;
}

[Config(typeof(BenchmarkConfig))]
public class NullableStructTest
{
    private NullableTestClass? testClass;
    private NullableTestStruct testStruct;
    private NullableTestStruct? testStruct2;

    public NullableStructTest()
    {
        this.testClass = new(1, 2);
        this.testStruct = new(1, 2);
        this.testStruct2 = new(1, 2);
    }

    [Benchmark]
    public int Sum_Class()
    {
        return this.testClass.Sum();
    }

    [Benchmark]
    public int? Sum_NullableClass()
    {
        return this.testClass?.Sum();
    }

    [Benchmark]
    public int Sum_Struct()
    {
        return this.testStruct.Sum();
    }

    [Benchmark]
    public int? Sum_NullableStruct()
    {
        return this.testStruct2?.Sum();
    }

    [Benchmark]
    public NullableTestPair Sum_ClassPair()
    {
        var pair = new NullableTestPair(this.testClass!, 1);
        return pair;
    }

    [Benchmark]
    public int? Sum_InterfaceStruct()
    {
        var @interface = (INullableTestInterface?)this.testStruct2;
        return @interface?.Sum();
    }
}
