// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class LfToCrLfTest
{
    private const string Text1 = "Hello, World!";
    private const string Text2 = "abc\r\n123\r\ntext";
    private const string Text3 = "abc\n123\ntext\n\n123\n\nabc";
    private string[] results = new string[3];

    public LfToCrLfTest()
    {
    }

    // [Benchmark]
    public string[] Replace()
    {
        this.results[0] = Text1.Replace("\n", "\r\n");
        this.results[1] = Text2.Replace("\n", "\r\n");
        this.results[2] = Text3.Replace("\n", "\r\n");
        return this.results;
    }

    [Benchmark]
    public string[] LfToCrLf()
    {
        this.results[0] = ConvertLfToCrLf(Text1);
        this.results[1] = ConvertLfToCrLf(Text2);
        this.results[2] = ConvertLfToCrLf(Text3);
        return this.results;
    }

    public static string ConvertLfToCrLf(string text)
    {
        var extra = 0;
        for (var i = 0; i < text.Length; i++)
        {
            if (text[i] == '\n' && (i == 0 || text[i - 1] != '\r'))
            {
                extra++;
            }
        }

        if (extra == 0)
        {
            return text;
        }

        return string.Create(text.Length + extra, text, static (dest, source) =>
        {
            var position = 0;
            for (var i = 0; i < source.Length; i++)
            {
                char c = source[i];
                if (c == '\n' && (i == 0 || source[i - 1] != '\r'))
                {
                    dest[position++] = '\r';
                    dest[position++] = '\n';
                }
                else
                {
                    dest[position++] = c;
                }
            }
        });
    }
}
