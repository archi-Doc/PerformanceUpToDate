// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public record class FindTestClass(int Id)
{
    public static implicit operator FindTestClass(int id)
    {
        return new(id);
    }
}

[Config(typeof(BenchmarkConfig))]
public class FindTest
{
    private readonly List<FindTestClass> list = new();
    private readonly int targetId = 6;

    public FindTest()
    {
        this.list = [1, 2, 3, 4, 5, 6, 7, 8,];
    }

    [Benchmark]
    public FindTestClass? Find()
    {
        return this.list.Find(x => x.Id == this.targetId);
    }

    [Benchmark]
    public FindTestClass? ForEach()
    {
        foreach (var x in this.list)
        {
            if (x.Id == this.targetId)
            {
                return x;
            }
        }

        return default;
    }

    [Benchmark]
    public FindTestClass? Marshal()
    {
        var span = CollectionsMarshal.AsSpan(this.list);
        for (var i = 0; i < span.Length; i++)
        {
            var c = span[i];
            if (c.Id == targetId)
            {
                return c;
            }
        }

        return null;
    }
}
