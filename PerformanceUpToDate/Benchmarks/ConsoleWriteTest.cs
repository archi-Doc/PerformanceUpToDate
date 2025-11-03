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
    public bool KeyAvailable()
    {
        return Console.KeyAvailable;
    }

    [Benchmark]
    public void String()
    {
        Console.Write(this.testString);
    }

    [Benchmark]
    public void String2()
    {
        Console.Write(this.testString);
        if (Console.CursorLeft > 0)
        {
            Console.CursorLeft--;
        }
    }

    /*[Benchmark]
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

    [Benchmark]
    public void ReadOnlySpan2()
    {
        var st = this.testString.AsSpan(0, 5);
        var st2 = this.testString.AsSpan(5);
        Console.Out.Write(st);
        Console.Out.Write(st2);
    }*/

    /*[Benchmark]
    public void WriteLine()
    {
        Console.WriteLine("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
    }

    [Benchmark]
    public void WriteLine2()
    {
        Console.WriteLine("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789\r\n012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
    }

    [Benchmark]
    public void WriteLine3()
    {
        Console.WriteLine("012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789");
        if (Console.CursorTop > 0)
        {
            Console.CursorTop--;
        }

        Console.WriteLine("a\u001b[K");
    }*/
}
