// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class IndentSpaceTest
{
    private readonly char[] spaceBuffer;

    public IndentSpaceTest()
    {
        this.spaceBuffer = new char[100];
        Array.Fill(this.spaceBuffer, ' ');
    }

    [Benchmark]
    public StringBuilder AppendRepeat()
    {
        var sb = new StringBuilder();
        sb.Append(' ', 100);
        return sb;
    }

    [Benchmark]
    public StringBuilder AppendBuffer()
    {
        var sb = new StringBuilder();
        sb.Append(this.spaceBuffer);
        return sb;
    }
}
