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
    public class ByteToULongTest
    {
        public byte[] ByteArray = default!;
        public ulong[] ULongArray = default!;

        public ByteToULongTest()
        {
            this.ByteArray = new byte[32];
            for (var i = 0; i < this.ByteArray.Length; i++)
            {
                this.ByteArray[i] = (byte)i;
            }

            this.ULongArray = this.ByteToULong_BlockCopy();
            var b = this.ULongToByte_BlockCopy();
            Debug.Assert(b.SequenceEqual(this.ByteArray), string.Empty);

            Debug.Assert(this.ULongArray.SequenceEqual(this.ByteToULong_BitConverter()), string.Empty);
            Debug.Assert(this.ULongArray.SequenceEqual(this.ByteToULong_BitConverter2()), string.Empty);
            Debug.Assert(this.ULongArray.SequenceEqual(this.ByteToULong_Unsafe()), string.Empty);
            Debug.Assert(this.ULongArray.SequenceEqual(this.ByteToULong_Unsafe2()), string.Empty);

            Debug.Assert(this.ByteArray.SequenceEqual(this.ULongToByte_BitConverter()), string.Empty);
            Debug.Assert(this.ByteArray.SequenceEqual(this.ULongToByte_Unsafe2()), string.Empty);
        }

        [GlobalSetup]
        public void Setup()
        {
        }

        [Benchmark]
        public ulong[] ByteToULong_BlockCopy()
        {
            var d = new ulong[4];
            Buffer.BlockCopy(this.ByteArray, 0, d, 0, 32);
            return d;
        }

        [Benchmark]
        public unsafe ulong[] ByteToULong_Unsafe()
        {
            var d = new ulong[4];
            fixed (ulong* pd = d)
            fixed (byte* pb = this.ByteArray)
            {
                ulong* dd = pd;
                byte* ss = pb;
                *dd++ = ((ulong)*ss++) | ((ulong)*ss++ << 8) | ((ulong)*ss++ << 16) | ((ulong)*ss++ << 24) |
                    ((ulong)*ss++ << 32) | ((ulong)*ss++ << 40) | ((ulong)*ss++ << 48) | ((ulong)*ss++ << 56);
                *dd++ = ((ulong)*ss++) | ((ulong)*ss++ << 8) | ((ulong)*ss++ << 16) | ((ulong)*ss++ << 24) |
                    ((ulong)*ss++ << 32) | ((ulong)*ss++ << 40) | ((ulong)*ss++ << 48) | ((ulong)*ss++ << 56);
                *dd++ = ((ulong)*ss++) | ((ulong)*ss++ << 8) | ((ulong)*ss++ << 16) | ((ulong)*ss++ << 24) |
                    ((ulong)*ss++ << 32) | ((ulong)*ss++ << 40) | ((ulong)*ss++ << 48) | ((ulong)*ss++ << 56);
                *dd++ = ((ulong)*ss++) | ((ulong)*ss++ << 8) | ((ulong)*ss++ << 16) | ((ulong)*ss++ << 24) |
                    ((ulong)*ss++ << 32) | ((ulong)*ss++ << 40) | ((ulong)*ss++ << 48) | ((ulong)*ss++ << 56);
            }

            return d;
        }

        [Benchmark]
        public unsafe ulong[] ByteToULong_Unsafe2()
        {
            var d = new ulong[4];
            fixed (ulong* pd = d)
            fixed (byte* pb = this.ByteArray)
            {
                ulong* dd = pd;
                ulong* ss = (ulong*)pb;
                *dd++ = *ss++;
                *dd++ = *ss++;
                *dd++ = *ss++;
                *dd++ = *ss++;
            }

            return d;
        }

        [Benchmark]
        public unsafe ulong[] ByteToULong_BitConverter()
        {
            var s = this.ByteArray.AsSpan();
            var d = new ulong[4];
            d[0] = BitConverter.ToUInt64(s);
            s = s.Slice(8);
            d[1] = BitConverter.ToUInt64(s);
            s = s.Slice(8);
            d[2] = BitConverter.ToUInt64(s);
            s = s.Slice(8);
            d[3] = BitConverter.ToUInt64(s);

            return d;
        }

        [Benchmark]
        public unsafe ulong[] ByteToULong_BitConverter2()
        {
            var d = new ulong[4];
            d[0] = BitConverter.ToUInt64(this.ByteArray, 0);
            d[1] = BitConverter.ToUInt64(this.ByteArray, 8);
            d[2] = BitConverter.ToUInt64(this.ByteArray, 16);
            d[3] = BitConverter.ToUInt64(this.ByteArray, 24);

            return d;
        }

        [Benchmark]
        public byte[] ULongToByte_BlockCopy()
        {
            var d = new byte[32];
            Buffer.BlockCopy(this.ULongArray, 0, d, 0, 32);
            return d;
        }

        [Benchmark]
        public unsafe byte[] ULongToByte_Unsafe2()
        {
            var d = new byte[32];
            fixed (byte* pd = d)
            fixed (ulong* pb = this.ULongArray)
            {
                ulong* dd = (ulong*)pd;
                ulong* ss = pb;
                *dd++ = *ss++;
                *dd++ = *ss++;
                *dd++ = *ss++;
                *dd++ = *ss++;
            }

            return d;
        }

        [Benchmark]
        public unsafe byte[] ULongToByte_BitConverter()
        {
            var d = new byte[32];
            var s = d.AsSpan();

            BitConverter.TryWriteBytes(s, this.ULongArray[0]);
            s = s.Slice(8);
            BitConverter.TryWriteBytes(s, this.ULongArray[1]);
            s = s.Slice(8);
            BitConverter.TryWriteBytes(s, this.ULongArray[2]);
            s = s.Slice(8);
            BitConverter.TryWriteBytes(s, this.ULongArray[3]);

            return d;
        }
    }
}
