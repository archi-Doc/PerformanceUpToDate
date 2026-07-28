// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using CommandLine.Text;
using Kimi.Compiler.Lexing;

namespace PerformanceUpToDate;

internal ref struct IndentWriter
{
    private StringBuilder? sb;

    public void Append(string value)
    {
        sb ??= new();
        sb.Append(value);
    }

    public void Append(char value)
    {
        sb ??= new();
        sb.Append(value);
    }

    public override string ToString()
    {
        return this.sb is null ? string.Empty : sb.ToString();
    }
}

[Config(typeof(BenchmarkConfig))]
public class WriteToTest
{
    private List<string> list = ["ABC", "12345", "asdfg"];

    public WriteToTest()
    {
    }

    //[Benchmark]
    public string StringJoin()
    {
        return $"alias {string.Join('.', this.list)}";
    }

    //[Benchmark]
    public string StringBuilder()
    {
        var sb = new StringBuilder();
        sb.Append("alias ");
        for (var i = 0; i < list.Count; i++)
        {
            sb.Append(this.list[i]);
            if (i < list.Count - 1)
            {
                sb.Append('.');
            }
        }

        return sb.ToString();
    }

    //[Benchmark]
    public string IndentWriter()
    {
        var sb = new IndentWriter();
        sb.Append("alias ");
        for (var i = 0; i < list.Count; i++)
        {
            sb.Append(this.list[i]);
            if (i < list.Count - 1)
            {
                sb.Append('.');
            }
        }

        return sb.ToString();
    }

    [Benchmark]
    public string SequenceBuilder()
    {
        // using var sb = new SequenceBuilder<char>();
        SequenceBuilder<char> sb = default;
        sb.AddRange("alias ");
        for (var i = 0; i < list.Count; i++)
        {
            sb.AddRange(this.list[i]);
            if (i < list.Count - 1)
            {
                sb.Add('.');
            }
        }

        var tx = sb.ToReadOnlySequence().ToString();
        sb.Dispose();
        return tx;
    }

    [Benchmark]
    public string RefStringBuilder()
    {
        using var sb = new RefStringBuilder();
        sb.AddRange("alias ");
        for (var i = 0; i < list.Count; i++)
        {
            sb.AddRange(this.list[i]);
            if (i < list.Count - 1)
            {
                sb.Add('.');
            }
        }

        var tx = sb.ToString();
        return tx;
    }
}
