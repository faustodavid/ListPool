using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using ListPool.Resolvers.Utf8Json;
using Utf8Json;

namespace ListPool.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class Utf8JsonDeserializeListOfIntBenchmarks
    {
        private static readonly ListPoolResolver _listPoolResolver = new ListPoolResolver();

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        private Stream _buffer;
        private Stream GetStream()
        {
            var stream = new MemoryStream();

            using var writer = new StreamWriter(stream, new UTF8Encoding(), leaveOpen: true);

            writer.Write("[1");
            writer.Flush();
            for (int i = 1; i < N; i++)
            {
                writer.Write($",{i}");
                writer.Flush();
            }

            writer.Write($"]");
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _buffer = GetStream();
        }

        [Benchmark]
        public async Task<int> ListPool_Pipes()
        {
            _buffer.Position = 0;
            using ListPool<int> list = await Utf8Json.JsonSerializer.DeserializeUsingPipesAsync(_buffer);
            return list.Count;
        }

        //[Benchmark(Baseline = true)]
        //public async Task<int> List_SystemTextJson()
        //{
        //    _buffer.Position = 0;
        //    List<int> list = await System.Text.Json.JsonSerializer.DeserializeAsync<List<int>>(_buffer);
        //    return list.Count;
        //}

      //  [Benchmark]
        public async Task<int> List_utf8json()
        {
            _buffer.Position = 0;
            List<int> list = await Utf8Json.JsonSerializer.DeserializeAsync<List<int>>(_buffer);
            return list.Count;
        }

        //[Benchmark]
        //public async Task<int> ListPool_utf8json()
        //{
        //    _buffer.Position = 0;
        //    using ListPool<int> list = await Utf8Json.JsonSerializer.DeserializeAsync<ListPool<int>>(_buffer);
        //    return list.Count;
        //}
        //[Benchmark]
        //public async Task<int> ListPool_SystemTextJson()
        //{
        //    _buffer.Position = 0;
        //    using ListPool<int> list = await System.Text.Json.JsonSerializer.DeserializeAsync<ListPool<int>>(_buffer);
        //    return list.Count;
        //}
      //  [Benchmark]
        public async Task<int> ListPool_utf8json_with_resolver()
        {
            _buffer.Position = 0;
            using ListPool<int> list = await JsonSerializer.DeserializeAsync<ListPool<int>>(_buffer, _listPoolResolver);
            return list.Count;
        }
    }
}
