﻿// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

/*  BenchmarkDotNet, small template code
 *  PM> Install-Package BenchmarkDotNet
 */

using System;
using System.Buffers;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using BigMachines;
using PerformanceUpToDate.BitTest;

namespace PerformanceUpToDate;

public class Program
{
    public static void Main(string[] args)
    {
        DebugRun<DIContainerBenchmark>();

        // var summary = BenchmarkRunner.Run<ByteCopyTest>(); // SwapTest, MemoryAllocationTest, ByteCopyTest
        var switcher = new BenchmarkSwitcher(new[]
#pragma warning restore SA1515 // Single-line comment should be preceded by blank line
        {
            typeof(DIContainerBenchmark),
            typeof(HashtableBenchmark),
            typeof(LargeStructTest),
            typeof(BytePoolTest),
            typeof(SizeOfBenchmark),
            typeof(DelegateOrGenericsBenchmark),
            typeof(MemoryMarshalTest),
            typeof(CubicRootTest),
            typeof(CancellationTokenTest),
            typeof(StructImplementation),
            typeof(StructInitializationTest),
            typeof(ByteArrayHashTest),
            typeof(CalcTest),
            typeof(TaskTest3),
            typeof(TaskTest2),
            typeof(EnumTest),
            typeof(ConcurrentTest),
            typeof(LockTest),
            typeof(DynamicAccessTest),
            typeof(AsyncEventTest),
            typeof(AsyncLocalTest),
            typeof(TaskTest),
            typeof(TimerTest),
            typeof(SyncDesignTest),
            typeof(ByteToULongTest),
            typeof(FillByteArrayTest),
            typeof(CopyIntArrayTest),
            typeof(FillIntArrayTest),
            typeof(SortComparerTest),
            typeof(IndexOfTest),
            typeof(BitTest.NLZ),
            typeof(RefTest.RefTest1),
            typeof(RefTest.RefTest2),
            typeof(RefTest.RefTest3),
            typeof(NewInstanceTest),
            typeof(NewInstanceTest2),
            typeof(StructTest),
            typeof(DelegateTest),
            typeof(MemoryAllocationTest),
            typeof(ByteCopyTest),
            typeof(ByteCompareTest),
            typeof(ByteCompareTest2),
            typeof(SpanTest),
            typeof(StreamTest),
            typeof(ImmutableTest),
            typeof(StringTest),
        });
        switcher.Run(args);
    }

    public static void DebugRun<T>()
        where T : new()
    { // Run a benchmark in debug mode.
        var t = new T();
        var type = typeof(T);
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var x in fields)
        { // Set Fields.
            var attr = (ParamsAttribute[])x.GetCustomAttributes(typeof(ParamsAttribute), false);
            if (attr != null && attr.Length > 0)
            {
                if (attr[0].Values.Length > 0)
                {
                    x.SetValue(t, attr[0].Values[0]);
                }
            }
        }

        foreach (var x in properties)
        { // Set Properties.
            var attr = (ParamsAttribute[])x.GetCustomAttributes(typeof(ParamsAttribute), false);
            if (attr != null && attr.Length > 0)
            {
                if (attr[0].Values.Length > 0)
                {
                    x.SetValue(t, attr[0].Values[0]);
                }
            }
        }

        foreach (var x in methods.Where(i => i.GetCustomAttributes(typeof(GlobalSetupAttribute), false).Length > 0))
        { // [GlobalSetupAttribute]
            x.Invoke(t, null);
        }

        foreach (var x in methods.Where(i => i.GetCustomAttributes(typeof(BenchmarkAttribute), false).Length > 0))
        { // [BenchmarkAttribute]
            x.Invoke(t, null);
        }

        foreach (var x in methods.Where(i => i.GetCustomAttributes(typeof(GlobalCleanupAttribute), false).Length > 0))
        { // [GlobalCleanupAttribute]
            x.Invoke(t, null);
        }

        // obsolete code:
        // methods.Where(i => i.CustomAttributes.Select(j => j.AttributeType).Contains(typeof(GlobalSetupAttribute)))
        // bool IsNullableType(Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        /* var targetType = IsNullableType(x.FieldType) ? Nullable.GetUnderlyingType(x.FieldType) : x.FieldType;
                    if (targetType != null)
                    {
                        var value = Convert.ChangeType(attr[0].Values[0], targetType);
                        x.SetValue(t, value);
                    }*/
    }
}

public class BenchmarkConfig : BenchmarkDotNet.Configs.ManualConfig
{
    public BenchmarkConfig()
    {
        this.AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
        this.AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default);

        // this.Add(Job.ShortRun.With(BenchmarkDotNet.Environments.Platform.X64).WithWarmupCount(1).WithIterationCount(1));
        // this.Add(BenchmarkDotNet.Jobs.Job.MediumRun.WithGcForce(true).WithId("GcForce medium"));
        // this.AddJob(BenchmarkDotNet.Jobs.Job.ShortRun);
        this.AddJob(BenchmarkDotNet.Jobs.Job.MediumRun);
    }
}
