using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class ListPoolClearBenchmarks
    {
        private int[] _fakeData;
        private List<int> _list;
        private ListPool<int> _listPool;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _fakeData = Enumerable.Range(0, N).Select(i => i).ToArray();
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _list.AddRange(_fakeData);
            _listPool.AddRange(_fakeData);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _list.Clear();
        }

        [Benchmark]
        public void ListPool()
        {
            _listPool.Clear();
        }
    }
}
