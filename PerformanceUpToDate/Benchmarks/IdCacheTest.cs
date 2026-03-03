// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using Arc.Crypto;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate.Internal;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public class IdCacheTest
{
    public const string Name = "PerformanceUpToDate.IdCacheTest";

    private readonly ThreadsafeTypeKeyHashtable<uint> typeToTypeIdentifier = new();

    public IdCacheTest()
    {
    }

    [Benchmark]
    public uint FarmHash64()
        => (uint)FarmHash.Hash64(Name);

    [Benchmark]
    public uint XxHash3_64()
        => (uint)XxHash3.Hash64(Name);

    [Benchmark]
    public uint IdCache_64()
        => IdCache<IdCacheTest>.Id;

    [Benchmark]
    public uint TypeKeyHashtable()
        => this.typeToTypeIdentifier.GetOrAdd(typeof(IdCacheTest), x => (uint)FarmHash.Hash64(x.FullName ?? string.Empty));

    private static class IdCache<T>
    {
        public static readonly uint Id;

        static IdCache()
        {
            Id = (uint)XxHash3.Hash64(typeof(T).FullName ?? string.Empty);
        }
    }
}
