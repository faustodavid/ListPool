using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ListPool.Utf8Json.Tests
{
    public class UnitTest1
    {
        private Stream _stream;

        public UnitTest1()
        {
            _stream = CreateStreamOfItems();
        }

        [Fact]
        public async Task Test1()
        {
            _stream.Position = 0;
            Stream stream = CreateStreamOfItems();

            ListPool<int> sut = await JsonSerializer.DeserializeUsingPipesAsync(_stream);

            Assert.Equal(10000, sut.Count);
        }

        [Fact]
        public async Task Test2()
        {
            _stream.Position = 0;
            Stream stream = CreateStreamOfItems();

            ListPool<int> sut = await JsonSerializer.DeserializeUsingPipesAsync(_stream);

            Assert.Equal(10000, sut.Count);
        }

        [Fact]
        public async Task Test3()
        {
            _stream.Position = 0;
            Stream stream = CreateStreamOfItems();

            ListPool<int> sut = await JsonSerializer.DeserializeUsingPipesAsync(_stream);

            Assert.Equal(10000, sut.Count);
        }

        private Stream CreateStreamOfItems()
        {
            var stream = new MemoryStream();

            using var writer = new StreamWriter(stream, new UTF8Encoding(), leaveOpen: true);

            writer.Write("[1");
            writer.Flush();
            for (int i = 1; i < 10000; i++)
            {
                writer.Write($",{i}");
                writer.Flush();
            }

            writer.Write($"]");
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
