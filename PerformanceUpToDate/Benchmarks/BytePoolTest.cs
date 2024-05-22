// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate.Design;

namespace PerformanceUpToDate;

public class ByteStack
{
    public ByteStack(int arrayLength, int stackLimit)
    {
        this.ArrayLength = arrayLength;
        this.StackLimit = stackLimit;
        this.stack = new byte[stackLimit][];
    }

    public int ArrayLength { get; }

    public int StackLimit { get; }

    public int Count
        => this.index;

    private byte[][]? stack;
    private int index;

    public byte[] Pop()
    {
        var original = this.DecrementIfPositive();
        if (original <= 0)
        {// No stack
            return new byte[this.ArrayLength];
        }
        else
        {
            return this.stack[original - 1];
        }
    }

    public void Push(byte[] byteArray)
    {
        var original = this.IncrementIfBelowLimit();
        if (original < this.StackLimit)
        {
            this.stack[original] = byteArray;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int DecrementIfPositive()
    {
        int original;
        do
        {
            original = this.index;
            if (original <= 0)
            {
                return original;
            }
        }
        while (Interlocked.CompareExchange(ref this.index, original - 1, original) != original);
        return original;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int IncrementIfBelowLimit()
    {
        int original;
        do
        {
            original = this.index;
            if (original >= this.StackLimit)
            {
                return original;
            }
        }
        while (Interlocked.CompareExchange(ref this.index, original + 1, original) != original);
        return original;
    }
}

[Config(typeof(BenchmarkConfig))]
public class BytePoolTest
{
    private const int N = 256; // 32

    private ConcurrentQueue<byte[]> concurrentQueue = new();
    private int count;
    private ByteStack byteStack = new(N, 4);
    private BlockingCollection<byte[]> blockingCollection = new(4);
    private CircularQueue<byte[]> circularQueue = new(4);

    public BytePoolTest()
    {
        var bin = this.byteStack.Pop();
        this.byteStack.Push(bin);
        this.byteStack.Pop();
        this.byteStack.Push(bin);

        if (!this.circularQueue.TryDequeue(out bin))
        {
            bin = new byte[N];
        }

        this.circularQueue.TryEnqueue(bin);
        if (!this.circularQueue.TryDequeue(out bin))
        {
            bin = new byte[N];
        }
    }

    [Benchmark]
    public byte[] NewByte()
    {
        return new byte[N];
    }

    [Benchmark]
    public byte[] ConcurrentQueue()
    {// ConcurrentQueue
        byte[] byteArray;
        if (!this.concurrentQueue.TryDequeue(out byteArray))
        {
            byteArray = new byte[N];
        }

        this.concurrentQueue.Enqueue(byteArray);
        return byteArray;
    }

    [Benchmark]
    public byte[] ConcurrentQueue2()
    {// ConcurrentQueue + Interlocked
        byte[] byteArray;
        if (this.concurrentQueue.TryDequeue(out byteArray))
        {
            Interlocked.Decrement(ref this.count);
        }
        else
        {
            byteArray = new byte[N];
        }

        this.concurrentQueue.Enqueue(byteArray);
        Interlocked.Increment(ref this.count);
        return byteArray;
    }

    [Benchmark]
    public (byte[] ByteArray, int Count) ConcurrentQueue3()
    {// ConcurrentQueue + Count
        byte[] byteArray;
        if (this.concurrentQueue.TryDequeue(out byteArray))
        {
        }
        else
        {
            byteArray = new byte[N];
        }

        this.concurrentQueue.Enqueue(byteArray);
        return (byteArray, this.concurrentQueue.Count);
    }

    /*[Benchmark]
    public byte[] ByteStack()
    {// ByteStack -> NOT thread-safe
        var byteArray = this.byteStack.Pop();
        this.byteStack.Push(byteArray);
        return byteArray;
    }*/

    /*[Benchmark]
    public byte[] BlockingCollection()
    {// BlockingCollection -> Slow
        byte[] byteArray;
        if (this.blockingCollection.TryTake(out byteArray))
        {
        }
        else
        {
            byteArray = new byte[N];
        }

        this.blockingCollection.TryAdd(byteArray);
        return byteArray;
    }*/

    [Benchmark]
    public byte[] CircularQueue()
    {// CircularQueue
        byte[] byteArray;
        if (!this.circularQueue.TryDequeue(out byteArray))
        {
            byteArray = new byte[N];
        }

        this.circularQueue.TryEnqueue(byteArray);
        return byteArray;
    }
}
