// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FastExpressionCompiler;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable SA1000

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class DynamicAccessTest
{
    public class Class
    {
        public int Id { get; private set; }

        public int Id2;
    }

    public Class ClassInstance { get; set; } = new();

    private Action<Class, int> setMethodExpressionTree;
    private Action<Class, int> setMethodExpressionTreeFast;
    private Action<Class, int> setMethodDelegate;
    private MethodInfo setMethodInfo;
    private FieldInfo fieldInfo;

    public DynamicAccessTest()
    {
        this.setMethodExpressionTree = this.CreateExpressionTree();
        this.setMethodExpressionTreeFast = this.CreateExpressionTreeFast();
        this.setMethodDelegate = this.CreateDelegate();
        this.setMethodInfo = this.CreateMethodInfo();
        this.fieldInfo = this.CreateFieldInfo();

        var c = new Class();
        this.setMethodExpressionTree(c, 1);
        this.setMethodExpressionTreeFast(c, 2);
        this.setMethodDelegate(c, 3);
        this.setMethodInfo.Invoke(c, new object[] { 4, });
        this.fieldInfo.SetValue(c, 3);
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public Action<Class, int> CreateExpressionTree()
    {
        var type = typeof(Class);
        var expType = Expression.Parameter(type);
        var mi = type.GetProperty("Id"/*, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic*/).GetSetMethod(true);
        var exp = Expression.Parameter(typeof(int));
        return Expression.Lambda<Action<Class, int>>(Expression.Call(expType, mi!, exp), expType, exp).Compile();
    }

    [Benchmark]
    public Action<Class, int> CreateExpressionTree2()
    {
        var type = typeof(Class);
        var expType = Expression.Parameter(type);
        var mi = type.GetMethod("set_Id", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var exp = Expression.Parameter(typeof(int));
        return Expression.Lambda<Action<Class, int>>(Expression.Call(expType, mi!, exp), expType, exp).Compile();
    }

    [Benchmark]
    public Action<Class, int> CreateExpressionTreeFast()
    {
        var type = typeof(Class);
        var expType = Expression.Parameter(type);
        var mi = type.GetMethod("set_Id", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var exp = Expression.Parameter(typeof(int));
        return Expression.Lambda<Action<Class, int>>(Expression.Call(expType, mi!, exp), expType, exp).CompileFast();
    }

    [Benchmark]
    public Action<Class, int> CreateDelegate()
    {
        var mi = typeof(Class).GetProperty("Id").GetSetMethod(true)!;
        return (Action<Class, int>)Delegate.CreateDelegate(typeof(Action<Class, int>), mi);
    }

    [Benchmark]
    public MethodInfo CreateMethodInfo()
    {
        var mi = typeof(Class).GetProperty("Id").GetSetMethod(true)!;
        return mi;
    }

    [Benchmark]
    public FieldInfo CreateFieldInfo()
    {
        return typeof(Class).GetField("Id2");
    }

    [Benchmark]
    public Class SetExpressionTree()
    {
        this.setMethodExpressionTree(this.ClassInstance, 1);
        return this.ClassInstance;
    }

    [Benchmark]
    public Class SetExpressionTreeFast()
    {
        this.setMethodExpressionTreeFast(this.ClassInstance, 1);
        return this.ClassInstance;
    }

    [Benchmark]
    public Class SetDelegate()
    {
        this.setMethodDelegate(this.ClassInstance, 1);
        return this.ClassInstance;
    }

    [Benchmark]
    public Class SetInvoke()
    {
        this.setMethodInfo.Invoke(this.ClassInstance, new object[] { 1, });
        return this.ClassInstance;
    }

    [Benchmark]
    public Class SetField()
    {
        this.fieldInfo.SetValue(this.ClassInstance, 4);
        return this.ClassInstance;
    }
}
