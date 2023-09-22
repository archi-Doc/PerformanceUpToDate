// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate.NewInstance2;

public class SimpleNewClass
{
    public SimpleNewClass()
    {
    }

    public int X { get; set; } = 49;

    public string Text { get; set; } = "Test";
}

public class NewConstraintClass : INewClass
{
    public NewConstraintClass()
    {
    }

    public int X { get; set; } = 49;

    public string Text { get; set; } = "Test";

    public static INewClass New() => new NewConstraintClass();
}

public interface INewClass
{
    static abstract INewClass New();
}

[Config(typeof(BenchmarkConfig))]
public class NewInstanceTest2
{
    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public SimpleNewClass SimpleNew()
        => new SimpleNewClass();

    [Benchmark]
    public SimpleNewClass ActivatorCreate()
        => Activator.CreateInstance<SimpleNewClass>();

    [Benchmark]
    public NewConstraintClass NewConstraint()
        => this.NewConstraintInternal<NewConstraintClass>();

    [Benchmark]
    public NewConstraintClass StaticAbstract()
        => this.StaticAbstractInternal<NewConstraintClass>();

    private T NewConstraintInternal<T>()
        where T : new()
    {
        return new T();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private T StaticAbstractInternal<T>()
        where T : INewClass
    {
        return (T)T.New();
    }
}
