// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Threading;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate;

namespace BigMachines;

public readonly struct RecursiveDetection
{
    public readonly ulong Id0;
    public readonly ulong Id1;
    public readonly ulong Id2;
    public readonly ulong Id3;
    public readonly ulong Id4;
    public readonly ulong Id5;

    public RecursiveDetection()
    {
    }

    public RecursiveDetection(ulong id0)
    {
        this.Id0 = id0;
    }

    public RecursiveDetection(ulong id0, ulong id1, ulong id2 = 0, ulong id3 = 0, ulong id4 = 0, ulong id5 = 0)
    {
        this.Id0 = id0;
        this.Id1 = id1;
    }

    public RecursiveDetection(ulong id0, ulong id1, ulong id2)
    {
        this.Id0 = id0;
        this.Id1 = id1;
        this.Id2 = id2;
    }

    public RecursiveDetection(ulong id0, ulong id1, ulong id2, ulong id3)
    {
        this.Id0 = id0;
        this.Id1 = id1;
        this.Id2 = id2;
        this.Id3 = id3;
    }

    public RecursiveDetection(ulong id0, ulong id1, ulong id2, ulong id3, ulong id4)
    {
        this.Id0 = id0;
        this.Id1 = id1;
        this.Id2 = id2;
        this.Id3 = id3;
        this.Id4 = id4;
    }
}

[Config(typeof(BenchmarkConfig))]
public class StructInitializationTest
{
    public StructInitializationTest()
    {
    }

    [Benchmark]
    public RecursiveDetection Default()
        => default;

    [Benchmark]
    public RecursiveDetection DefaultConstructor()
        => new RecursiveDetection();

    /*[Benchmark]
    public RecursiveDetection Constructor1()
        => new RecursiveDetection(123456);

    [Benchmark]
    public RecursiveDetection Constructor2B()
        => new RecursiveDetection(123456, 123456);*/

    [Benchmark]
    public RecursiveDetection Constructor4()
        => new RecursiveDetection(123456, 123456, 123456, 123456);
}
