// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class IndexOfTest2
{
    public const string Text = "123456789Hello World123456789Hello World123456789Hello World/#+Hello World";
    public const char Separator1 = '/';
    public const char Separator2 = '#';
    public const char Separator3 = '+';
    public const string Separators = "/abcfgijkmnp";

    private readonly SearchValues<char> searchValues = SearchValues.Create(Separators);

    public IndexOfTest2()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public int IndexOf()
        => Text.AsSpan().IndexOf(Separator1);

    [Benchmark]
    public int IndexOf2()
    {
        var span = Text.AsSpan();
        return span.IndexOf(Separator1) + span.IndexOf(Separator2);
    }

    [Benchmark]
    public int IndexOfAny2()
    {
        var span = Text.AsSpan();
        return span.IndexOfAny(Separator1, Separator2);
    }

    [Benchmark]
    public int IndexOfAny3()
    {
        var span = Text.AsSpan();
        return span.IndexOfAny(Separator1, Separator2, Separator3);
    }

    [Benchmark]
    public int IndexOfAnySpan()
    {
        var span = Text.AsSpan();
        return span.IndexOfAny(Separators);
    }

    [Benchmark]
    public int IndexOfAnySearchValue()
    {
        var span = Text.AsSpan();
        return span.IndexOfAny(this.searchValues);
    }

    [Benchmark]
    public int Span()
    {
        var span = Text.AsSpan();
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] == Separator1 || span[i] == Separator2 || span[i] == Separator3)
            {
                return i;
            }
        }

        return -1;
    }
}
