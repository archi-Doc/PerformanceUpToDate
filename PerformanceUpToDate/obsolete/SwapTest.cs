// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1307

namespace PerformanceUpToDate.Obsolete;

[Config(typeof(BenchmarkConfig))]
public class SwapTest
{
    private SwapClass swap;

    public SwapTest()
    {
        this.swap = new SwapClass();
    }

    [Benchmark]
    public SwapClass Swap_Temp()
    {
        int temp;

        temp = this.swap.a;
        this.swap.a = this.swap.b;
        this.swap.b = temp;

        return this.swap;
    }

    [Benchmark]
    public SwapClass Swap_Tuple()
    {
        (this.swap.a, this.swap.b) = (this.swap.b, this.swap.a);

        return this.swap;
    }

    public class SwapClass
    {
        public int a = 0;
        public int b = 1;
        public int c = 2;
    }
}
