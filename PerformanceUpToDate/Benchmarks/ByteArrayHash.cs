// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace PerformanceUpToDate.BitTest;

[Config(typeof(BenchmarkConfig))]
public class ByteArrayHashTest
{
    public static byte[] NewByteArray(int n)
    {
        var byteArray = new byte[n];
        for (var i = 0; i < n; i++)
        {
            byteArray[i] = (byte)i;
        }

        return byteArray;
    }

    public static unsafe int ByteArrayCode(byte[] bytes)
    {
        var length = bytes.Length;
        if (length == 0)
        {
            return HashCode.Combine(length);
        }
        else if (length == 1)
        {
            int i = bytes[0];
            return HashCode.Combine(length, i);
        }
        else if (length == 2)
        {
            int i = (bytes[1] << 8) | bytes[0];
            return HashCode.Combine(length, i);
        }
        else if (length == 3)
        {
            int i = (bytes[2] << 16) | (bytes[1] << 8) | bytes[0];
            return HashCode.Combine(length, i);
        }
        else
        {
            fixed (byte* b = bytes)
            {
                int* first = (int*)b;
                int* last = (int*)(b + length - 4);

                return HashCode.Combine(length, first[0], last[0]);
            }
        }
    }

    public static int GetStringHashCode(string str)
    {
        return ByteArrayCode(Encoding.UTF8.GetBytes(str));
    }

    public byte[] ByteArray_10 { get; set; }

    public byte[] ByteArray_100 { get; set; }

    public byte[] ByteArray_1000 { get; set; }

    public ByteArrayHashTest()
    {
        this.ByteArray_10 = NewByteArray(10);
        this.ByteArray_100 = NewByteArray(100);
        this.ByteArray_1000 = NewByteArray(1000);

        int hash = 0;
        hash = GetStringHashCode("");
        hash = GetStringHashCode("1");
        hash = GetStringHashCode("2");
        hash = GetStringHashCode("11");
        hash = GetStringHashCode("22");
        hash = GetStringHashCode("1111");
        hash = GetStringHashCode("11111");
        hash = GetStringHashCode("11111");
        hash = GetStringHashCode("11112");
        hash = GetStringHashCode("21111");
    }

    [GlobalSetup]
    public void Setup()
    {
    }

    /*[Benchmark]
    public int ByteArray_10_GetHashCode()
    {
        return this.ByteArray_10.GetHashCode();
    }

    [Benchmark]
    public int ByteArray_100_GetHashCode()
    {
        return this.ByteArray_100.GetHashCode();
    }

    [Benchmark]
    public int ByteArray_1000_GetHashCode()
    {
        return this.ByteArray_1000.GetHashCode();
    }*/

    [Benchmark]
    public int ByteArray_10_AddBytes()
    {
        var hc = default(HashCode);
        hc.AddBytes(this.ByteArray_10);
        return hc.ToHashCode();
    }

    [Benchmark]
    public int ByteArray_100_AddBytes()
    {
        var hc = default(HashCode);
        hc.AddBytes(this.ByteArray_100);
        return hc.ToHashCode();
    }

    [Benchmark]
    public int ByteArray_1000_AddBytes()
    {
        var hc = default(HashCode);
        hc.AddBytes(this.ByteArray_1000);
        return hc.ToHashCode();
    }

    [Benchmark]
    public int ByteArray_10_FarmHash64()
        => (int)Arc.Crypto.FarmHash.Hash64(this.ByteArray_10);

    [Benchmark]
    public int ByteArray_100_FarmHash64()
        => (int)Arc.Crypto.FarmHash.Hash64(this.ByteArray_100);

    [Benchmark]
    public int ByteArray_1000_FarmHash64()
        => (int)Arc.Crypto.FarmHash.Hash64(this.ByteArray_1000);

    [Benchmark]
    public int ByteArray_10_Original()
        => ByteArrayCode(this.ByteArray_10);

    [Benchmark]
    public int ByteArray_100_Original()
        => ByteArrayCode(this.ByteArray_100);

    [Benchmark]
    public int ByteArray_1000_Original()
        => ByteArrayCode(this.ByteArray_1000);
}
