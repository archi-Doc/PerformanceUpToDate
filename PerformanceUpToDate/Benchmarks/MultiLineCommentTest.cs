// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class MultiLineCommentTest
{
    public const string Comment = """
        /* var x = 1 * 2;
           Method1();
           Method2();
           private readonly SearchValues<char> searchValues = SearchValues.Create(Separators);
           var x = 2 * 3;
           Method3();
           Method4();
        */
        """;

    public const char Separator1 = '*';
    public const char Separator2 = '\n';
    public const string Separators = "*\n";

    private readonly SearchValues<char> searchValues = SearchValues.Create(Separators);

    public MultiLineCommentTest()
    {
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public int IndexOfAny()
    {
        var span = Comment.AsSpan();
        var i = 2;
        while (i < span.Length)
        {
            i += span.Slice(i).IndexOfAny(Separator1, Separator2);
            if (span[i + 1] == '/')
            {
                break;
            }

            i++;
        }

        return i;
    }

    [Benchmark]
    public int LastIndexOf()
    {
        var i = Comment.AsSpan().IndexOf("*/");
        var span = Comment.AsSpan(0, i);
        i += span.LastIndexOf(Separator2);
        i += span.Count(Separator2);

        return i;
    }
}
