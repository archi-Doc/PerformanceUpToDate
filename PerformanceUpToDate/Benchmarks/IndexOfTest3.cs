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
public class IndexOfTest3
{
    public const string Text = "123456789Hello World123456789Hello*World123456789Hello World/#+Hello World";
    public const char Separator1 = '*';
    public const char Separator2 = '\n';
    public const string Separators = "*\n";

    private readonly SearchValues<char> searchValues = SearchValues.Create(Separators);

    public IndexOfTest3()
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
    public int IndexOfAny()
        => Text.AsSpan().IndexOfAny(Separator1, Separator2);

    [Benchmark]
    public int SearchValue()
        => Text.AsSpan().IndexOfAny(searchValues);
}
