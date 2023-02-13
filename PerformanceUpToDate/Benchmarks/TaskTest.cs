// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

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

        public async Task<int> TaskResponse3(bool sync)
        {
            if (sync)
            {
                return await this.TaskResponse2(sync);
            }
            else
            {
                await Task.Yield();
                return await this.TaskResponse2(sync);
            }
        }

        public async Task<int> TaskResponse3b(bool sync)
        {
            return this.TaskResponse2b(sync); // return new Task
        }

        public int TaskResponse2b(bool sync)
        {
            return this.TaskResponse(sync).Result;
        }

        public Task<int> TaskResponse3c(bool sync)
        {
            return Task.Run(() => this.TaskResponse2b(sync)); // return new Task
        }

        public async Task<int> TaskResponse2(bool sync)
        {
            if (sync)
            {
                return await this.TaskResponse(sync);
            }
            else
            {
                await Task.Yield();
                return await this.TaskResponse(sync);
            }
        }

        public async Task<int> TaskWaitResponse2(bool sync)
        {
            if (sync)
            {
                return this.TaskResponse(sync).Result;
            }
            else
            {
                await Task.Yield();
                return this.TaskResponse(sync).Result;
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
                return await this.ValueTaskResponse(sync);
            }
            else
            {
                await Task.Yield();
                return await this.ValueTaskResponse(sync);
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

    /*[Benchmark]
    public async Task<int> TaskWithoutAwait()
    {
        return await this.Class1.TaskResponse(true);
    }

    [Benchmark]
    public int TaskWaitWithoutAwait()
    {
        return this.Class1.TaskResponse(true).Result;
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
    }*/

    [Benchmark]
    public async Task<int> Task3()
    {
        return await this.Class1.TaskResponse3(true);
    }

    [Benchmark]
    public async Task<int> Task3b()
    {
        return await this.Class1.TaskResponse3b(true);
    }

    [Benchmark]
    public async Task<int> Task3c()
    {
        return await this.Class1.TaskResponse3c(true);
    }
}
