// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace PerformanceUpToDate;

public class SimpleTransientClass
{
    public SimpleTransientClass()
    {
    }

    public int X { get; set; } = 49;

    public string Text { get; set; } = "Test";
}

public class SimpleScopedClass
{
    public SimpleScopedClass()
    {
    }

    public int X { get; set; } = 49;

    public string Text { get; set; } = "Test";
}

public class SimpleSingletonClass
{
    public SimpleSingletonClass()
    {
    }

    public int X { get; set; } = 49;

    public string Text { get; set; } = "Test";
}

[Config(typeof(BenchmarkConfig))]
public class DIContainerBenchmark
{
    private readonly IServiceProvider serviceProvider;
    private readonly IServiceScope serviceScope;

    public DIContainerBenchmark()
    {
        var sc = new ServiceCollection();
        sc.AddTransient<SimpleTransientClass>();
        sc.AddScoped<SimpleScopedClass>();
        sc.AddSingleton<SimpleSingletonClass>();

        this.serviceProvider = sc.BuildServiceProvider();
        this.serviceScope = this.serviceProvider.CreateScope();
    }

    [Benchmark]
    public SimpleTransientClass New()
        => new SimpleTransientClass();

    [Benchmark]
    public SimpleTransientClass Transient()
        => this.serviceProvider.GetRequiredService<SimpleTransientClass>();

    [Benchmark]
    public SimpleScopedClass Scoped()
        => this.serviceScope.ServiceProvider.GetRequiredService<SimpleScopedClass>();

    [Benchmark]
    public SimpleScopedClass Scoped2()
    {
        using (var scope = this.serviceProvider.CreateScope())
        {
            return scope.ServiceProvider.GetRequiredService<SimpleScopedClass>();
        }
    }

    [Benchmark]
    public SimpleSingletonClass Singleton()
        => this.serviceProvider.GetRequiredService<SimpleSingletonClass>();
}
