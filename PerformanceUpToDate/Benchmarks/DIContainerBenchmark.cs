// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using BenchmarkDotNet.Attributes;
using DryIoc;
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

    private readonly Container container;
    private readonly IResolverContext serviceScope2;

    public DIContainerBenchmark()
    {
        var sc = new ServiceCollection();
        sc.AddTransient<SimpleTransientClass>();
        sc.AddScoped<SimpleScopedClass>();
        sc.AddSingleton<SimpleSingletonClass>();

        this.serviceProvider = sc.BuildServiceProvider();
        this.serviceScope = this.serviceProvider.CreateScope();

        this.container = new DryIoc.Container();
        this.container.Register<SimpleTransientClass>(Reuse.Transient);
        this.container.Register<SimpleScopedClass>(Reuse.Scoped);
        this.container.Register<SimpleSingletonClass>(Reuse.Singleton);
        this.serviceScope2 = this.container.OpenScope();
    }

    [Benchmark]
    public SimpleTransientClass New()
        => new SimpleTransientClass();

    [Benchmark]
    public SimpleTransientClass Transient_Ms()
        => this.serviceProvider.GetRequiredService<SimpleTransientClass>();

    [Benchmark]
    public SimpleTransientClass Transient_Dry()
        => this.container.Resolve<SimpleTransientClass>();

    [Benchmark]
    public SimpleScopedClass Scoped_Ms()
        => this.serviceScope.ServiceProvider.GetRequiredService<SimpleScopedClass>();

    [Benchmark]
    public SimpleScopedClass Scoped_Dry()
        => this.serviceScope2.Resolve<SimpleScopedClass>();

    [Benchmark]
    public SimpleScopedClass Scoped2_Ms()
    {
        using (var scope = this.serviceProvider.CreateScope())
        {
            return scope.ServiceProvider.GetRequiredService<SimpleScopedClass>();
        }
    }

    [Benchmark]
    public SimpleScopedClass Scoped2_Dry()
    {
        using (var scope = this.container.OpenScope())
        {
            return scope.Resolve<SimpleScopedClass>();
        }
    }

    [Benchmark]
    public SimpleSingletonClass Singleton_Ms()
        => this.serviceProvider.GetRequiredService<SimpleSingletonClass>();

    [Benchmark]
    public SimpleSingletonClass Singleton_Dry()
        => this.container.Resolve<SimpleSingletonClass>();
}
