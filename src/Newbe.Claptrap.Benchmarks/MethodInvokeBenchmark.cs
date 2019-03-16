using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using BenchmarkDotNet.Attributes;

namespace Newbe.Claptrap.Benchmarks
{
    public class MethodInvokeBenchmark
    {
        public class TestClass
        {
            public TestClass GetOne()
            {
                return this;
            }
        }

        /// <summary>
        /// call in times
        /// </summary>
        [Params(10, 100, 1000, 5000, 10000)]
        public int Times { get; set; }

        private TestClass _instance;
        private MethodInfo _methodInfo;
        private Func<TestClass> _func;
        private IReadOnlyDictionary<string, Func<TestClass, TestClass>> _table;
        private dynamic _dInstance;
        private static TestClass StaticInvoke(TestClass testClass)
        {
            return testClass.GetOne();
        }

        [GlobalSetup]
        public void Setup()
        {
            _instance = new TestClass();
            _methodInfo = typeof(TestClass).GetMethod(nameof(TestClass.GetOne));
            _func = _instance.GetOne;
            _table
                = new ReadOnlyDictionary<string, Func<TestClass, TestClass>>(
                    new Dictionary<string, Func<TestClass, TestClass>>
                    {
                        {"getOne", StaticInvoke}
                    });
            _dInstance = _instance;
        }

        [Benchmark(Baseline = true)]
        public TestClass Directly()
        {
            TestClass re = null;
            for (var i = 0; i < Times; i++)
            {
                re = _instance.GetOne();
            }

            return re;
        }

        [Benchmark]
        public TestClass TableFunc()
        {
            TestClass re = null;
            for (var i = 0; i < Times; i++)
            {
                var func = _table["getOne"];
                re = func(_instance);
            }

            return re;
        }

        [Benchmark]
        public TestClass Reflection()
        {
            TestClass re = null;
            for (var i = 0; i < Times; i++)
            {
                re = (TestClass) _methodInfo.Invoke(_instance, null);
            }

            return re;
        }

        [Benchmark]
        public TestClass Func()
        {
            TestClass re = null;
            for (var i = 0; i < Times; i++)
            {
                re = _func.Invoke();
            }

            return re;
        }
        
        [Benchmark]
        public TestClass Dynamic()
        {
            TestClass re = null;
            for (var i = 0; i < Times; i++)
            {
                re = _dInstance.GetOne();
            }

            return re;
        }
    }
}