﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using ListPool.Serializers.Utf8Json.Resolvers;
using Utf8Json;

namespace ListPool.Benchmarks.Serializers
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [GcServer(true)]
    [GcConcurrent]
    public class Deserialize_List_Int
    {
        private static readonly ListPoolResolver _listPoolResolver = new ListPoolResolver();

        private Stream _buffer;

        [Params(100, 1_000, 10_000)]
        public int N { get; set; }

        private Stream GetStream()
        {
            MemoryStream stream = new MemoryStream();

            using StreamWriter writer = new StreamWriter(stream, new UTF8Encoding(), leaveOpen: true);

            writer.Write("[1");
            writer.Flush();
            for (int i = 1; i < N; i++)
            {
                writer.Write($",{i}");
                writer.Flush();
            }

            writer.Write("]");
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _buffer = GetStream();
        }

        [Benchmark(Baseline = true)]
        public async Task<int> Utf8Json_List()
        {
            _buffer.Position = 0;
            List<int> list = await JsonSerializer.DeserializeAsync<List<int>>(_buffer, _listPoolResolver);
            return list.Count;
        }

        [Benchmark]
        public async Task<int> Utf8Json_ListPool()
        {
            _buffer.Position = 0;
            using ListPool<int> list = await JsonSerializer.DeserializeAsync<ListPool<int>>(_buffer);
            return list.Count;
        }

        [Benchmark]
        public async Task<int> Utf8Json_ListPool_with_resolver()
        {
            _buffer.Position = 0;
            using ListPool<int> list = await JsonSerializer.DeserializeAsync<ListPool<int>>(_buffer, _listPoolResolver);
            return list.Count;
        }

        [Benchmark]
        public async Task<int> STJ_List()
        {
            _buffer.Position = 0;
            List<int> list = await System.Text.Json.JsonSerializer.DeserializeAsync<List<int>>(_buffer);
            return list.Count;
        }

        [Benchmark]
        public async Task<int> STJ_ListPool()
        {
            _buffer.Position = 0;
            using ListPool<int> list = await System.Text.Json.JsonSerializer.DeserializeAsync<ListPool<int>>(_buffer);
            return list.Count;
        }
    }
}
