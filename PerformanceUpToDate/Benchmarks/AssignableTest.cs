// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public interface IAssignableTest1
{
}

public interface IAssignableTest2 : IAssignableTest1
{
}

public class AssignableTestClass1
{
}

public class AssignableTestClass2 : IAssignableTest1
{
}

public class AssignableTestClass3 : IAssignableTest2
{
}

[Config(typeof(BenchmarkConfig))]
public class AssignableTest
{
    public AssignableTest()
    {
    }

    private bool IsAssinable1(Type type)
    {
        return type.IsAssignableTo(typeof(IAssignableTest1));
    }

    private bool IsAssinable2(Type type)
    {
        return type.IsAssignableTo(typeof(IAssignableTest2));
    }

    [Benchmark]
    public bool Test_Class3_1()
    {
        return this.IsAssinable1(typeof(AssignableTestClass3));
    }

    [Benchmark]
    public bool Test_Class3_2()
    {
        return this.IsAssinable2(typeof(AssignableTestClass3));
    }

    [Benchmark]
    public bool Test_object_bool()
    {
        return typeof(object).IsAssignableTo(typeof(bool));
    }

    [Benchmark]
    public bool Test_Class1_Test1()
    {
        return typeof(AssignableTestClass1).IsAssignableTo(typeof(IAssignableTest1));
    }

    [Benchmark]
    public bool Test_Class2_Test1()
    {
        return typeof(AssignableTestClass2).IsAssignableTo(typeof(IAssignableTest1));
    }

    [Benchmark]
    public bool Test_Class3_Test1()
    {
        return typeof(AssignableTestClass3).IsAssignableTo(typeof(IAssignableTest1));
    }

    [Benchmark]
    public bool Test_Class3_Test2()
    {
        return typeof(AssignableTestClass3).IsAssignableTo(typeof(IAssignableTest2));
    }
}
