// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate.Internal;

#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate;

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
    private ThreadsafeTypeKeyHashtable<Func<object>> constructors = new();

    public NewInstanceTest2()
    {
        this.constructors.TryAdd(typeof(SimpleNewClass), () => new SimpleNewClass());
    }

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
    public SimpleNewClass ActivatorCreate2()
        => (SimpleNewClass)Activator.CreateInstance(typeof(SimpleNewClass));

    [Benchmark]
    public SimpleNewClass TypeKeyHashtable()
       => this.constructors.TryGetValue(typeof(SimpleNewClass), out var func) ? (SimpleNewClass)func() : default!;

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
