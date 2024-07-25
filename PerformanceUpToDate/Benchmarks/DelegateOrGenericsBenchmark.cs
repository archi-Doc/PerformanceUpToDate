using System;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

public interface IGetInstance
{
    object GetInstance();
}

public class DelegateOrGenericsClass : IGetInstance
{
    public DelegateOrGenericsClass()
    {
        this.GetDelegate = static x => x.instance;
    }

    object IGetInstance.GetInstance()
        => this.instance;

    private object instance = new();

    public Func<DelegateOrGenericsClass, object> GetDelegate { get; private set; }
}

[Config(typeof(BenchmarkConfig))]
public class DelegateOrGenericsBenchmark
{
    public DelegateOrGenericsBenchmark()
    {
        this.tc = new();
        this.tc2 = this.tc;
    }

    private DelegateOrGenericsClass tc;
    private object tc2;

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public object Get_Generics()
        => this.Get(this.tc);

    [Benchmark]
    public object Get_Interface()
        => ((IGetInstance)this.tc2).GetInstance();

    [Benchmark]
    public object Get_Delegate()
        => this.tc.GetDelegate(this.tc);

    private object Get<T>(T obj)
        where T : IGetInstance
    {
        return obj.GetInstance();
    }
}
