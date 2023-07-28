// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class TaskTest2
{
    public class TaskClass
    {
        public TaskClass(int x)
        {
            this.x = x;
        }

        public async Task<bool> TaskTrue()
        {
            return true;
        }

        public async ValueTask<bool> ValueTaskTrue()
        {
            return true;
        }

        public async Task<bool> TaskBool(bool value)
        {
            return value;
        }

        public async ValueTask<bool> ValueTaskBool(bool value)
        {
            return value;
        }

        public async Task<int> Task1()
        {
            return 1;
        }

        public async ValueTask<int> ValueTask1()
        {
            return 1;
        }

        public async Task<int> Task20()
        {
            return 20;
        }

        public async ValueTask<int> ValueTask20()
        {
            return 20;
        }

        public async Task<int> TaskInt(int value)
        {
            return value + this.x;
        }

        public async ValueTask<int> ValueTaskInt(int value)
        {
            return value + this.x;
        }

        private int x;
    }

    public TaskClass Class1 { get; }

    private int x;

    public TaskTest2()
    {
        this.Class1 = new TaskClass(1);
        this.x = 33;
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public async Task<bool> TaskTrue()
        => await this.Class1.TaskTrue();

    [Benchmark]
    public async ValueTask<bool> ValueTaskTrue()
        => await this.Class1.ValueTaskTrue();

    [Benchmark]
    public async Task<bool> TaskBool()
        => await this.Class1.TaskBool(true);

    [Benchmark]
    public async ValueTask<bool> ValueTaskBool()
        => await this.Class1.ValueTaskBool(true);

    [Benchmark]
    public async Task<int> Task1()
        => await this.Class1.Task1();

    [Benchmark]
    public async ValueTask<int> ValueTask1()
        => await this.Class1.ValueTask1();

    [Benchmark]
    public async Task<int> Task20()
        => await this.Class1.Task20();

    [Benchmark]
    public async ValueTask<int> ValueTask20()
        => await this.Class1.ValueTask20();

    [Benchmark]
    public async Task<int> TaskInt()
        => await this.Class1.TaskInt(this.x);

    [Benchmark]
    public async ValueTask<int> ValueTaskInt()
        => await this.Class1.ValueTaskInt(this.x);
}
