// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public struct RefArrayStruct
{
    public RefArrayStruct()
    {
    }

    public int X;
    public long Y;
    public long Z;
}

[Config(typeof(BenchmarkConfig))]
public class RefArrayTest
{
    private RefArrayStruct[] array = new RefArrayStruct[1];

    public RefArrayTest()
    {
    }

    [Benchmark]
    public long SetIndex()
    {
        this.array[0].X = 1;
        this.array[0].Y = 1;
        this.array[0].Z = 1;
        return this.array[0].Z;
    }

    [Benchmark]
    public long SetRef()
    {
        ref var item = ref this.array[0];
        item.X = 1;
        item.Y = 1;
        item.Z = 1;
        return item.Z;
    }
}
