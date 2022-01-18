﻿// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class TaskTest
{
    public class TaskClass
    {
        public TaskClass(int x)
        {
            this.x = x;
        }

        public async Task<int> TaskResponse(bool sync)
        {
            if (sync)
            {
                return this.x;
            }
            else
            {
                await Task.Yield();
                return this.x;
            }
        }

        public async Task<int> TaskResponse2(bool sync)
        {
            if (sync)
            {
                return await TaskResponse(sync);
            }
            else
            {
                await Task.Yield();
                return await TaskResponse(sync);
            }
        }

        public async ValueTask<int> ValueTaskResponse(bool sync)
        {
            if (sync)
            {
                return this.x;
            }
            else
            {
                await Task.Yield();
                return this.x;
            }
        }

        public async ValueTask<int> ValueTaskResponse2(bool sync)
        {
            if (sync)
            {
                return await ValueTaskResponse(sync);
            }
            else
            {
                await Task.Yield();
                return await ValueTaskResponse(sync);
            }
        }

        private int x;
    }

    public TaskClass Class1 { get; }

    public TaskTest()
    {
        this.Class1 = new TaskClass(1);
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public async Task<int> TaskWithoutAwait()
    {
        return await this.Class1.TaskResponse(true);
    }

    [Benchmark]
    public async Task<int> TaskToValueTaskWithoutAwait2()
    {
        var vt = new ValueTask<int>(this.Class1.TaskResponse(true));
        return await vt;
    }

    [Benchmark]
    public async ValueTask<int> ValueTaskWithoutAwait()
    {
        return await this.Class1.ValueTaskResponse(true);
    }

    [Benchmark]
    public async Task<int> TaskWithAwait()
    {
        return await this.Class1.TaskResponse(false);
    }

    [Benchmark]
    public async ValueTask<int> ValueTaskWithAwait()
    {
        return await this.Class1.ValueTaskResponse(false);
    }

    [Benchmark]
    public async Task<int> Task2WithoutAwait()
    {
        return await this.Class1.TaskResponse2(true);
    }

    [Benchmark]
    public async ValueTask<int> ValueTask2WithoutAwait()
    {
        return await this.Class1.ValueTaskResponse2(true);
    }
}