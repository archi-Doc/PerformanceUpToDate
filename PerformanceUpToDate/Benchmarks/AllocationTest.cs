// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate;

[Config(typeof(BenchmarkConfig))]
public partial class AllocationTest
{
    private const int Size = 32;
    private const uint ZeroMemory = 0x00000008;

    private readonly nint heap;

    /*[DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetProcessHeap();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint HeapAlloc(nint hHeap, uint dwFlags, nuint dwBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool HeapFree(nint hHeap, uint dwFlags, nint lpMem);*/

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial IntPtr GetProcessHeap();

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial IntPtr HeapAlloc(
        IntPtr hHeap,
        uint dwFlags,
        nuint dwBytes);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool HeapFree(
        IntPtr hHeap,
        uint dwFlags,
        IntPtr lpMem);

    public AllocationTest()
    {
        this.heap = GetProcessHeap();
    }

    [Benchmark]
    public byte[] NewByte()
    {
        return new byte[Size];
    }

    [Benchmark]
    public nint GlobalAlloc()
    {
        var p = Marshal.AllocHGlobal(Size);
        Marshal.FreeHGlobal(p);
        return p;
    }

    [Benchmark]
    public unsafe void NativeAlloc()
    {
        void* p = NativeMemory.Alloc(Size);
        NativeMemory.Free(p);
    }

    [Benchmark]
    public nint HeapAlloc()
    {
        var p = HeapAlloc(this.heap, 0, Size);
        HeapFree(heap, 0, p);
        return p;
    }
}
