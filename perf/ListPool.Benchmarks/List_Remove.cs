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
    public class List_Remove
    {
        private List<int> _list;
        private ListPool<int> _listPool;
        private int[] _fakeData;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _list = new List<int>(N);
            _listPool = new ListPool<int>(N);
            _fakeData = Enumerable.Range(0, N).Select(i => i).ToArray();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _list.AddRange(_fakeData);
            _listPool.AddRange(_fakeData);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            _list.Clear();
            _listPool.Clear();
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _listPool.Dispose();
        }

        [Benchmark(Baseline = true)]
        public void List()
        {
            _list.Remove(N / 2);
        }

        [Benchmark]
        public void ListPool()
        {
            _listPool.Remove(N / 2);
        }
    }
}
