// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Buffers;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1401 // Fields should be private

namespace PerformanceUpToDate
{
    [Config(typeof(BenchmarkConfig))]
    public class StreamTest
    {
        public byte[] byteArray = default!;
        public MemoryStream memoryStream = default!;
        public Stream stream = default!;

        [GlobalSetup]
        public void Setup()
        {
            this.byteArray = new byte[10000];
            this.memoryStream = new MemoryStream();
            this.memoryStream.Write(this.byteArray.AsSpan());
            this.stream = this.memoryStream;
        }

        [Benchmark]
        public void ToArray()
        {
            this.memoryStream.Position = 0;
            this.memoryStream.ToArray().AsSpan();
        }

        [Benchmark]
        public void ArrayPool()
        {
            this.memoryStream.Position = 0;
            var buffer = ArrayPool<byte>.Shared.Rent(checked((int)this.memoryStream.Length));
            this.stream.Read(buffer);
            var span = buffer.AsSpan();
            ArrayPool<byte>.Shared.Return(buffer);
            return;
        }

        [Benchmark]
        public void TryGetBuffer()
        {
            this.memoryStream.Position = 0;
            if (this.stream is MemoryStream ms && ms.TryGetBuffer(out ArraySegment<byte> streamBuffer))
            {
                var span = streamBuffer.AsSpan(checked((int)ms.Position));
                ms.Seek(span.Length, SeekOrigin.Current);
                return;
            }

            this.memoryStream.ToArray().AsSpan();
            return;
        }
    }
}
