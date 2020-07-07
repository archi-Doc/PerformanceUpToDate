// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1204 // Static elements should appear before instance elements

namespace PerformanceUpToDate
{
    [Config(typeof(BenchmarkConfig))]
    public class ByteCompareTest
    {
        private byte[] source;
        private byte[] destination;

        public ByteCompareTest()
        {
            this.source = null!;
            this.destination = null!;

            this.Setup();
        }

        public ByteCompareTest(int size)
        {
            this.source = null!;
            this.destination = null!;

            this.Size = size;
            this.Setup();
        }

        // [Params(10, 32, 256, 1024, 1_000_000)]
        [Params(10,  256, 2048)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            this.source = new byte[this.Size];
            this.destination = new byte[this.Size];

            new Random(42).NextBytes(this.source);
            Array.Copy(this.source, 0, this.destination, 0, this.source.Length);
        }

        [Benchmark]
        public bool ByteCompare_IStructuralEquatable()
        {
            return (this.source as IStructuralEquatable).Equals(this.destination, StructuralComparisons.StructuralEqualityComparer);
        }

        [Benchmark]
        public bool ByteCompare_SequenceEqual()
        {
            return Enumerable.SequenceEqual(this.source, this.destination);
        }

        [Benchmark]
        public bool ByteCompare_SequenceEqualSpan()
        {
            return this.source.AsSpan().SequenceEqual(this.destination);
        }

        [Benchmark]
        public bool ByteCompare_For()
        {
            if (this.source.Length != this.destination.Length)
            {
                return false;
            }

            for (int i = 0; i < this.source.Length; i++)
            {
                if (this.source[i] != this.destination[i])
                {
                    return false;
                }
            }

            return true;
        }

        [Benchmark]
        public unsafe bool ByteCompare_int()
        {
            if (this.source.Length != this.destination.Length)
            {
                return false;
            }

            int len = this.source.Length;
            unsafe
            {
                fixed (byte* ap = this.source, bp = this.destination)
                {
                    int* aip = (int*)ap, bip = (int*)bp;
                    for (; len >= 4; len -= 4)
                    {
                        if (*aip != *bip)
                        {
                            return false;
                        }

                        aip++;
                        bip++;
                    }

                    byte* ap2 = (byte*)aip, bp2 = (byte*)bip;
                    for (; len > 0; len--)
                    {
                        if (*ap2 != *bp2)
                        {
                            return false;
                        }

                        ap2++;
                        bp2++;
                    }
                }
            }

            return true;
        }

        [Benchmark]
        public unsafe bool ByteCompare_int2()
        {
            if (this.source == this.destination)
            {
                return true;
            }

            if (this.source == null || this.destination == null)
            {
                return false;
            }

            if (this.source.Length != this.destination.Length)
            {
                return false;
            }

            int len = this.source.Length;
            fixed (byte* p1 = this.source, p2 = this.destination)
            {
                int* i1 = (int*)p1;
                int* i2 = (int*)p2;
                while (len >= 4)
                {
                    if (*i1 != *i2)
                    {
                        return false;
                    }

                    i1++;
                    i2++;
                    len -= 4;
                }

                byte* c1 = (byte*)i1;
                byte* c2 = (byte*)i2;
                while (len > 0)
                {
                    if (*c1 != *c2)
                    {
                        return false;
                    }

                    c1++;
                    c2++;
                    len--;
                }
            }

            return true;
        }

        [Benchmark]
        public unsafe bool ByteCompare_int10()
        {
            int length = this.source.Length;
            if (length != this.destination.Length)
            {
                return false;
            }

            fixed (byte* str = this.source)
            {
                byte* chPtr = str;
                fixed (byte* str2 = this.destination)
                {
                    byte* chPtr2 = str2;
                    byte* chPtr3 = chPtr;
                    byte* chPtr4 = chPtr2;
                    while (length >= 10)
                    {
                        if ((((*((int*)chPtr3) != *((int*)chPtr4)) || (*((int*)(chPtr3 + 2)) != *((int*)(chPtr4 + 2)))) || ((*((int*)(chPtr3 + 4)) != *((int*)(chPtr4 + 4))) || (*((int*)(chPtr3 + 6)) != *((int*)(chPtr4 + 6))))) || (*((int*)(chPtr3 + 8)) != *((int*)(chPtr4 + 8))))
                        {
                            break;
                        }

                        chPtr3 += 10;
                        chPtr4 += 10;
                        length -= 10;
                    }

                    while (length > 0)
                    {
                        if (*((int*)chPtr3) != *((int*)chPtr4))
                        {
                            break;
                        }

                        chPtr3 += 2;
                        chPtr4 += 2;
                        length -= 2;
                    }

                    return length <= 0;
                }
            }
        }

        [Benchmark]
        public unsafe bool ByteCompare_intshortbyte()
        {
            if (this.source == null || this.destination == null || this.source.Length != this.destination.Length)
            {
                return false;
            }

            fixed (byte* p1 = this.source, p2 = this.destination)
            {
                byte* x1 = p1, x2 = p2;
                int l = this.source.Length;
                for (int i = 0; i < l / 8; i++, x1 += 8, x2 += 8)
                {
                    if (*((long*)x1) != *((long*)x2))
                    {
                        return false;
                    }
                }

                if ((l & 4) != 0)
                {
                    if (*((int*)x1) != *((int*)x2))
                    {
                        return false;
                    }

                    x1 += 4;
                    x2 += 4;
                }

                if ((l & 2) != 0)
                {
                    if (*((short*)x1) != *((short*)x2))
                    {
                        return false;
                    }

                    x1 += 2;
                    x2 += 2;
                }

                if ((l & 1) != 0)
                {
                    if (*((byte*)x1) != *((byte*)x2))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [Benchmark]
        public unsafe bool ByteCompare_long()
        {
            if (this.source.Length != this.destination.Length)
            {
                return false;
            }

            int len = this.source.Length;
            unsafe
            {
                fixed (byte* ap = this.source, bp = this.destination)
                {
                    long* alp = (long*)ap, blp = (long*)bp;
                    for (; len >= 8; len -= 8)
                    {
                        if (*alp != *blp)
                        {
                            return false;
                        }

                        alp++;
                        blp++;
                    }

                    byte* ap2 = (byte*)alp, bp2 = (byte*)blp;
                    for (; len > 0; len--)
                    {
                        if (*ap2 != *bp2)
                        {
                            return false;
                        }

                        ap2++;
                        bp2++;
                    }
                }
            }

            return true;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public static extern int memcmp(byte[] b1, byte[] b2, long count);
#pragma warning restore SA1300 // Element should begin with upper-case letter

        [Benchmark]
        public bool ByteCompare_memcmp()
        {
            return this.source.Length == this.destination.Length && memcmp(this.source, this.destination, this.source.Length) == 0;
        }

        public void Test()
        {
            Debug.Assert(this.ByteCompare_IStructuralEquatable(), "ByteCompare_IStructuralEquatable");
            Debug.Assert(this.ByteCompare_SequenceEqual(), "ByteCompare_SequenceEqual");
            Debug.Assert(this.ByteCompare_For(), "ByteCompare_For");
            Debug.Assert(this.ByteCompare_int(), "ByteCompare_int");
            Debug.Assert(this.ByteCompare_int2(), "ByteCompare_int2");
            Debug.Assert(this.ByteCompare_int10(), "ByteCompare_int10");
            Debug.Assert(this.ByteCompare_intshortbyte(), "ByteCompare_intshortbyte");
            Debug.Assert(this.ByteCompare_long(), "ByteCompare_long");
            Debug.Assert(this.ByteCompare_memcmp(), "ByteCompare_memcmp");
        }
    }
}
