// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CpoySpanTest
{
    private const int Length = 20;
    private const string EraseLineAndLfString = "\u001b[K\n";
    private const string EraseLineAndCrLfString = "\u001b[K\r\n";
    private readonly char[] buffer = new char[Length];

    public static ReadOnlySpan<char> EraseLineAndLfSpan => "\u001b[K\n";

    public static ReadOnlySpan<char> EraseLineAndCrLfSpan => "\u001b[K\r\n";

    public ReadOnlySpan<char> EraseLineAndNewLineSpan => Environment.NewLine == "\r\n" ? "\u001b[K\r\n" : "\u001b[K\n";

    public static ReadOnlySpan<char> EraseLineAndNewLineSpan2 => Environment.NewLine == "\r\n" ? "\u001b[K\r\n" : "\u001b[K\n";

    public char[] EraseLineAndNewLine { get; }

    public CpoySpanTest()
    {
        if (Environment.NewLine == "\r\n")
        {
            this.EraseLineAndNewLine = ['\u001b', '[', 'K', '\r', '\n',];
        }
        else
        {
            this.EraseLineAndNewLine = ['\u001b', '[', 'K', '\n',];
        }
    }

    /*[Benchmark]
    public int CopyToSpan()
    {
        Span<char> span = stackalloc char[Length];
        EraseLineAndLfString.CopyTo(span);
        return span.Length;
    }

    [Benchmark]
    [SkipLocalsInit]
    public int CopyToSpan2()
    {
        Span<char> span = stackalloc char[Length];
        EraseLineAndLfString.CopyTo(span);
        return span.Length;
    }

    [Benchmark]
    public int CopyToArray()
    {
        EraseLineAndLfString.CopyTo(this.buffer.AsSpan());
        return this.buffer.Length;
    }*/

    [Benchmark]
    [SkipLocalsInit]
    public int CommonToSpan()
    {
        Span<char> span = stackalloc char[Length];
        EraseLineAndNewLineSpan.CopyTo(span);
        return span.Length;
    }

    [Benchmark]
    [SkipLocalsInit]
    public int CommonToSpan2()
    {
        Span<char> span = stackalloc char[Length];
        this.EraseLineAndNewLine.AsSpan().CopyTo(span);
        return span.Length;
    }

    [Benchmark]
    [SkipLocalsInit]
    public int CommonToSpan3()
    {
        Span<char> span = stackalloc char[Length];
        EraseLineAndNewLineSpan2.CopyTo(span);
        return span.Length;
    }
}
