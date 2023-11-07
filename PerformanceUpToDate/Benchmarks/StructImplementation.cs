// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Threading;
using Arc.Crypto;
using BenchmarkDotNet.Attributes;
using PerformanceUpToDate;

namespace BigMachines;

public readonly partial struct DualAddress
{// 28 bytes
    public readonly ushort Engagement4;
    public readonly ushort Engagement6;
    public readonly ushort Port4;
    public readonly ushort Port6;
    public readonly uint Address4;
    public readonly ulong Address6A;
    public readonly ulong Address6B;

    public DualAddress(ushort port4, uint address4, ushort port6, ulong address6a, ulong address6b)
    {
        this.Port4 = port4;
        this.Port6 = port6;
        this.Address4 = address4;
        this.Address6A = address6a;
        this.Address6B = address6b;
    }
}

public readonly partial record struct DualAddressRecord
{// 28 bytes
    public readonly ushort Engagement4;
    public readonly ushort Engagement6;
    public readonly ushort Port4;
    public readonly ushort Port6;
    public readonly uint Address4;
    public readonly ulong Address6A;
    public readonly ulong Address6B;

    public DualAddressRecord(ushort port4, uint address4, ushort port6, ulong address6a, ulong address6b)
    {
        this.Port4 = port4;
        this.Port6 = port6;
        this.Address4 = address4;
        this.Address6A = address6a;
        this.Address6B = address6b;
    }
}

public readonly partial struct DualAddressImplemented : IEquatable<DualAddressImplemented>
{// 28 bytes
    public readonly ushort Engagement4;
    public readonly ushort Engagement6;
    public readonly ushort Port4;
    public readonly ushort Port6;
    public readonly uint Address4;
    public readonly ulong Address6A;
    public readonly ulong Address6B;

    public DualAddressImplemented(ushort port4, uint address4, ushort port6, ulong address6a, ulong address6b)
    {
        this.Port4 = port4;
        this.Port6 = port6;
        this.Address4 = address4;
        this.Address6A = address6a;
        this.Address6B = address6b;
    }

    public bool Equals(DualAddressImplemented other)
        => this.Engagement4 == other.Engagement4 &&
        this.Engagement6 == other.Engagement6 &&
        this.Port4 == other.Port4 &&
        this.Port6 == other.Port6 &&
        this.Address4 == other.Address4 &&
        this.Address6A == other.Address6A &&
        this.Address6B == other.Address6B;

    public override int GetHashCode()
        => HashCode.Combine(this.Engagement4, this.Engagement6, this.Port4, this.Port6, this.Address4, this.Address6A, this.Address6B);
}

public readonly partial struct DualAddressRapid : IEquatable<DualAddressRapid>
{// 28 bytes
    public readonly ushort Engagement4;
    public readonly ushort Engagement6;
    public readonly ushort Port4;
    public readonly ushort Port6;
    public readonly uint Address4;
    public readonly ulong Address6A;
    public readonly ulong Address6B;

    public DualAddressRapid(ushort port4, uint address4, ushort port6, ulong address6a, ulong address6b)
    {
        this.Port4 = port4;
        this.Port6 = port6;
        this.Address4 = address4;
        this.Address6A = address6a;
        this.Address6B = address6b;
    }

    public unsafe bool Equals(DualAddressRapid other)
    {
        fixed (void* p = &this)
        {
            return new ReadOnlySpan<byte>(p, sizeof(DualAddressRapid)).SequenceEqual(new ReadOnlySpan<byte>(&other, sizeof(DualAddressRapid)));
        }
    }

    public unsafe override int GetHashCode()
    {
        fixed (void* p = &this)
        {
            return (int)FarmHash.Hash64(new ReadOnlySpan<byte>(p, sizeof(DualAddressRapid)));
        }
    }
}

[Config(typeof(BenchmarkConfig))]
public class StructImplementation
{
    public StructImplementation()
    {
        this.dualAddress = new(1, 22, 33, 4444, 555555);
        this.dualAddressImplemented = new(1, 22, 33, 4444, 555555);
        this.dualAddressRecord = new(1, 22, 33, 4444, 555555);
        this.dualAddressRapid = new(1, 22, 33, 4444, 555555);

        this.dualAddress2 = this.dualAddress;
        this.dualAddressImplemented2 = this.dualAddressImplemented;
        this.dualAddressRecord2 = this.dualAddressRecord;
        this.dualAddressRapid2 = this.dualAddressRapid;
    }

    private readonly DualAddress dualAddress;
    private readonly DualAddress dualAddress2;
    private readonly DualAddressImplemented dualAddressImplemented;
    private readonly DualAddressImplemented dualAddressImplemented2;
    private readonly DualAddressRecord dualAddressRecord;
    private readonly DualAddressRecord dualAddressRecord2;
    private readonly DualAddressRapid dualAddressRapid;
    private readonly DualAddressRapid dualAddressRapid2;

    [Benchmark]
    public bool Equals_DualAddress()
    {
        var result = this.dualAddress.Equals(this.dualAddress2);
        return result;
    }

    [Benchmark]
    public bool Equals_DualAddressImplemented()
    {
        var result = this.dualAddressImplemented.Equals(this.dualAddressImplemented2);
        return result;
    }

    [Benchmark]
    public bool Equals_DualAddressRecord()
    {
        var result = this.dualAddressRecord.Equals(this.dualAddressRecord2);
        return result;
    }

    [Benchmark]
    public bool Equals_DualAddressRapid()
    {
        var result = this.dualAddressRapid.Equals(this.dualAddressRapid2);
        return result;
    }

    [Benchmark]
    public int GetHashCode_DualAddress()
    {
        var result = this.dualAddress.GetHashCode();
        return result;
    }

    [Benchmark]
    public int GetHashCode_DualAddressImplemented()
    {
        var result = this.dualAddressImplemented.GetHashCode();
        return result;
    }

    [Benchmark]
    public int GetHashCode_DualAddressRecord()
    {
        var result = this.dualAddressRecord.GetHashCode();
        return result;
    }

    [Benchmark]
    public int GetHashCode_DualAddressRapid()
    {
        var result = this.dualAddressRapid.GetHashCode();
        return result;
    }
}
