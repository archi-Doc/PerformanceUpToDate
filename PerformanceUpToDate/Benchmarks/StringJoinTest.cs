// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class StringJoinTest
{
    private readonly string[] strings;

    public StringJoinTest()
    {
        this.strings = ["Test", "1", "abcdefghij", "1234567890", "123", "Test", "1", "abcdefghij",];

        var st = this.StringJoin();
        var st2 = this.StringBuilder();
    }

    [Benchmark]
    public string StringJoin()
    {
        return string.Join(' ', this.strings);
    }

    [Benchmark]
    public string StringBuilder()
    {
        var sb = new StringBuilder();
        foreach (var x in this.strings)
        {
            if (sb.Length > 0)
            {
                sb.Append(' ');
            }

            sb.Append(x);
        }

        return sb.ToString();
    }
}
