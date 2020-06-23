// Copyright (c) All contributors. All rights reserved. Licensed under the MIT license.

using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;

#pragma warning disable SA1649 // File name should match first type name

namespace PerformanceUpToDate
{
    public struct DelegateKey
    {
        public Type InstanceType;
        public MethodInfo Method;

        public DelegateKey(Type instanceType, MethodInfo method)
        {
            this.InstanceType = instanceType;
            this.Method = method;
        }

        public override int GetHashCode() => HashCode.Combine(this.InstanceType, this.Method);

        public int GetHashCode_XOR() => this.InstanceType.GetHashCode() ^ this.Method.GetHashCode();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var x = (DelegateKey)obj;
            return this.InstanceType.Equals(x.InstanceType) && this.Method.Equals(x.Method);
        }

        public bool Equals2(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(DelegateKey))
            {
                return false;
            }

            var x = (DelegateKey)obj;
            return this.InstanceType.Equals(x.InstanceType) && this.Method.Equals(x.Method);
        }

        public bool Equals3(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(DelegateKey))
            {
                return false;
            }

            var x = (DelegateKey)obj;
            return this.InstanceType == x.InstanceType && this.Method == x.Method;
        }

        public bool Equals4(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }

            var x = (DelegateKey)obj;
            return this.InstanceType == x.InstanceType && this.Method == x.Method;
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class StructTest
    {
        private DelegateKey key;
        private DelegateKey key2;

        [GlobalSetup]
        public void Setup()
        {
            Action action = () => { };
            this.key.InstanceType = typeof(StructTest);
            this.key.Method = action.Method;
            this.key2.InstanceType = typeof(StructTest);
            this.key2.Method = action.Method;
        }

        [Benchmark]
        public bool Equals()
        {
            return this.key.Equals(this.key2);
        }

        [Benchmark]
        public bool Equals2()
        {
            return this.key.Equals2(this.key2);
        }

        [Benchmark]
        public bool Equals3()
        {
            return this.key.Equals3(this.key2);
        }

        [Benchmark]
        public bool Equals4()
        {
            return this.key.Equals4(this.key2);
        }

        [Benchmark]
        public new int GetHashCode() => this.key.GetHashCode();

        [Benchmark]
        public int GetHashCode_XOR() => this.key.GetHashCode_XOR();
    }
}
