// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class CountTripleQuotesTest
{
    private readonly string testString = "\"\"\"1234 Test string\n1234 Test string\"\"\" 1234 Test string1234 Test string1234 Test string1234 Test string1234 Test stringTest stringTest string\"\"\" ";

    public CountTripleQuotesTest()
    {
        int count;
        count = CountRaw(this.testString);
        count = CountIndexOf(this.testString);
    }

    [Benchmark]
    public int TestRaw()
    {
        return CountRaw(this.testString);
    }

    [Benchmark]
    public int TestIndexOf()
    {
        return CountIndexOf(this.testString);
    }

    public static unsafe int CountRaw(ReadOnlySpan<char> text)
    {
        var length = text.Length;
        var count = 0;
        if (length < 3)
        {
            return 0;
        }

        fixed (char* p = text)
        {
            char* ptr = p;
            char* end = p + length - 2;

            while (ptr <= end)
            {
                if (ptr[0] == '\"' && ptr[1] == '\"' && ptr[2] == '\"')
                {
                    count++;
                    ptr += 3;
                }
                else
                {
                    ptr++;
                }
            }
        }

        return count;
    }

    public static unsafe int CountIndexOf(ReadOnlySpan<char> text)
    {
        ReadOnlySpan<char> tripleQuotes = "\"\"\"";

        var span = text;
        var count = 0;
        while (true)
        {
            var index = span.IndexOf(tripleQuotes);
            if (index == -1)
            {
                return count;
            }

            count++;
            span = span.Slice(index + tripleQuotes.Length);
        }
    }
}
