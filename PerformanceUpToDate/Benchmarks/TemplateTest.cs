// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class TemplateTest
{
    public TemplateTest()
    {
    }

    [Benchmark]
    public void Test1()
    {
    }
}
