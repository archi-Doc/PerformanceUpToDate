// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace PerformanceUpToDate
{
    [Config(typeof(BenchmarkConfig))]
    public class ByteCopyTest
    {
        private byte[] source;
        private byte[] destination;

        public ByteCopyTest()
        {
            this.source = new byte[this.Size];
            this.destination = new byte[this.Size];

            this.Setup();
        }

        // [Params(10, 1024, 1_000_000)]
        [Params(10, 20, 32, 256, 1024, 4_000, 32_000, 1_000_000)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            new Random(42).NextBytes(this.source);
        }

        [Benchmark]
        public byte[] ByteCopy_For()
        {
            for (var n = 0; n < this.source.Length; n++)
            {
                this.destination[n] = this.source[n];
            }

            return this.destination;
        }

        [Benchmark]
        public byte[] ByteCopy_ArrayCopy()
        {
            Array.Copy(this.source, 0, this.destination, 0, this.source.Length);
            return this.destination;
        }

        [Benchmark]
        public byte[] ByteCopy_BufferBlockCopy()
        {
            Buffer.BlockCopy(this.source, 0, this.destination, 0, this.source.Length);
            return this.destination;
        }

        [Benchmark]
        public byte[] ByteCopy_BufferMemoryCopy()
        {
            unsafe
            {
                fixed (void* s = this.source, d = this.destination)
                {
                    Buffer.MemoryCopy(s, d, this.destination.Length, this.source.Length);
                }
            }

            return this.destination;
        }

        [Benchmark]
        public byte[] ByteCopy_UnsafeLong()
        {
            int len = this.source.Length;
            unsafe
            {
                fixed (byte* s = this.source, d = this.destination)
                {
                    long* s2 = (long*)s, d2 = (long*)d;
                    for (; len >= 8; len -= 8)
                    {
                        *d2++ = *s2++;
                    }

                    byte* s3 = (byte*)s2, d3 = (byte*)d2;
                    for (; len > 0; len--)
                    {
                        *d3++ = *s3++;
                    }
                }
            }

            return this.destination;
        }

        public void Test()
        {
            this.Size = 1023;

            this.Setup();
            this.ByteCopy_For();
            Debug.Assert(this.source.SequenceEqual(this.destination), "For");

            this.Setup();
            this.ByteCopy_ArrayCopy();
            Debug.Assert(this.source.SequenceEqual(this.destination), "ArrayCopy");

            this.Setup();
            this.ByteCopy_BufferBlockCopy();
            Debug.Assert(this.source.SequenceEqual(this.destination), "BufferBlockCopy");

            this.Setup();
            this.ByteCopy_BufferMemoryCopy();
            Debug.Assert(this.source.SequenceEqual(this.destination), "BufferMemoryCopy");

            this.Setup();
            this.ByteCopy_UnsafeLong();
            Debug.Assert(this.source.SequenceEqual(this.destination), "UnsafeLong");
        }
    }
}
