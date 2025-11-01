// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class ConsoleWriteTest
{
    private readonly string testString = "Hello World!";
    public ConsoleWriteTest()
    {
    }

    [Benchmark]
    public void String()
    {
        Console.Write(this.testString);
    }

    [Benchmark]
    public void NewString()
    {
        var st = new string(this.testString);
        Console.Write(st);
    }

    [Benchmark]
    public void ReadOnlySpan()
    {
        var st = this.testString.AsSpan();
        Console.Out.Write(st);
    }
}
