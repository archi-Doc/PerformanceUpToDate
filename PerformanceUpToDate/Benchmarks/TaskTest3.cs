// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

#pragma warning disable CS0414

namespace PerformanceUpToDate;

public static class TaskCache
{
    static TaskCache()
    {
        TrueTask = Task.FromResult(true);
        FalseTask = Task.FromResult(false);
    }

    public static readonly Task<bool> TrueTask;
    public static readonly Task<bool> FalseTask;
}

[Config(typeof(BenchmarkConfig))]
public class TaskTest3
{
    private static readonly Task<TestEnum> TestEnumC = Task.FromResult(TestEnum.C);
    private static readonly ValueTask<TestEnum> TestEnumC2 = ValueTask.FromResult(TestEnum.C);

    public enum TestEnum
    {
        A,
        B,
        C,
    }

    public class TaskClass
    {
        public TaskClass(int x)
        {
            this.x = x;
        }

        public async Task<bool> TaskTrue2()
            => await this.TaskTrue();

        public Task<bool> TaskTrueB()
            => this.TaskTrue();

        public Task<bool> TaskTrueC()
            => Task.FromResult(true);

        public Task<bool> TaskTrueD()
            => TaskCache.TrueTask;

        public async ValueTask<bool> ValueTaskTrue2()
            => await this.ValueTaskTrue();

        public async Task<bool> TaskBool2(bool value)
            => await this.TaskBool(value);

        public Task<bool> TaskBoolB(bool value)
            => this.TaskBool(value);

        public async ValueTask<bool> ValueTaskBool2(bool value)
            => await this.ValueTaskBool(value);

        public Task<int> Task1_B()
            => this.Task1();

        public async Task<int> Task1_2()
            => await this.Task1() + 100;

        public async ValueTask<int> ValueTask1_2()
            => await this.ValueTask1() + 100;

        public async Task<int> Task20_2()
            => await this.Task20() + 100;

        public Task<int> Task20_B()
            => this.Task20();

        public async ValueTask<int> ValueTask20_2()
            => await this.ValueTask20() + 100;

        public async Task<int> TaskInt2(int value)
            => await this.TaskInt(value) + 100;

        public Task<int> TaskIntB(int value)
            => this.TaskInt(value + 100);

        public async ValueTask<int> ValueTaskInt2(int value)
            => await this.ValueTaskInt(value) + 100;

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

    public TaskTest3()
    {
        this.Class1 = new TaskClass(1);
        this.x = 33;
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public async Task<bool> TaskTrue2()
        => await this.Class1.TaskTrue2();

    [Benchmark]
    public async Task<bool> TaskTrueB()
        => await this.Class1.TaskTrueB();

    [Benchmark]
    public Task<bool> TaskTrueC()
        => this.Class1.TaskTrueC();

    [Benchmark]
    public Task<int> TaskInt1()
        => Task.FromResult(1);

    [Benchmark]
    public Task<int> TaskInt10000()
        => Task.FromResult(10000);

    [Benchmark]
    public ValueTask<int> ValueTaskInt1()
        => ValueTask.FromResult(1);

    [Benchmark]
    public ValueTask<int> ValueTaskInt10000()
        => ValueTask.FromResult(10000);

    [Benchmark]
    public Task<TaskClass> Task_Class()
        => Task.FromResult(this.Class1);

    [Benchmark]
    public ValueTask<TaskClass> ValueTask_Class()
        => ValueTask.FromResult(this.Class1);

    [Benchmark]
    public Task<TestEnum> Task_Enum()
        => Task.FromResult(TestEnum.C);

    [Benchmark]
    public ValueTask<TestEnum> ValueTask_Enum()
        => ValueTask.FromResult(TestEnum.C);

    [Benchmark]
    public Task<TestEnum> Task_Enum2()
        => TestEnumC;

    [Benchmark]
    public ValueTask<TestEnum> ValueTask_Enum2()
        => TestEnumC2;

    /*[Benchmark]
    public Task<bool> TaskTrueD()
        => this.Class1.TaskTrueD();

    [Benchmark]
    public async ValueTask<bool> ValueTaskTrue2()
        => await this.Class1.ValueTaskTrue2();

    [Benchmark]
    public async Task<bool> TaskBool2()
        => await this.Class1.TaskBool2(true);

    [Benchmark]
    public async Task<bool> TaskBoolB()
        => await this.Class1.TaskBoolB(true);

    [Benchmark]
    public async ValueTask<bool> ValueTaskBool2()
        => await this.Class1.ValueTaskBool2(true);

    [Benchmark]
    public async Task<int> Task1_2()
        => await this.Class1.Task1_2();

    [Benchmark]
    public async Task<int> Task1_B()
        => await this.Class1.Task1_B();

    [Benchmark]
    public async ValueTask<int> ValueTask1_2()
        => await this.Class1.ValueTask1_2();

    [Benchmark]
    public async Task<int> Task20_2()
        => await this.Class1.Task20_2();

    [Benchmark]
    public async Task<int> Task20_B()
        => await this.Class1.Task20_B();

    [Benchmark]
    public async ValueTask<int> ValueTask20_2()
        => await this.Class1.ValueTask20_2();

    [Benchmark]
    public async Task<int> TaskInt2()
        => await this.Class1.TaskInt2(this.x);

    [Benchmark]
    public async Task<int> TaskIntB()
        => await this.Class1.TaskIntB(this.x);

    [Benchmark]
    public async ValueTask<int> ValueTaskInt2()
        => await this.Class1.ValueTaskInt2(this.x);*/
}
