using Straight.Core.DataAccess.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization;
using Xunit;

namespace Straight.Core.DataAccess.Tests
{
    public class JSonDomainEventSerializerTests
    {
        public JSonDomainEventSerializerTests()
        {
            _serializer = new JSonEventSerializer();
        }

        private JSonEventSerializer _serializer;

        private static string GetReadToEnd(Stream stream)
        {
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }

        public class JSonDataTest
        {
            public string Content { get; set; }
            public int Integer { get; set; }
        }

        [Fact]
        public void Should_get_data_when_deserialize_data()
        {
            var jsonActual = "{\"Content\":\"Flash Gordon\",\"Integer\":10000}";
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(jsonActual);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                var data = _serializer.Deserialize<JSonDataTest>(new StreamReader(stream));
                Assert.NotNull(data);
                Assert.Equal(data.Content, "Flash Gordon");
                Assert.Equal(data.Integer, 10000);
            }
        }

        [Fact]
        public void Should_json_format_when_serialize_data()
        {
            var data = new JSonDataTest
            {
                Content = "Flash Gordon",
                Integer = 10000
            };
            string jsonActual = null;
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(new StreamWriter(stream), data);
                stream.Position = 0;
                jsonActual = new StreamReader(stream).ReadToEnd();
            }
            Assert.NotNull(jsonActual);
            Assert.NotEmpty(jsonActual);
            Assert.Equal(jsonActual, "{\"Content\":\"Flash Gordon\",\"Integer\":10000}" + Environment.NewLine);
        }

        [Fact]
        public void Should_throw_deserialization_exception_when_null_reference()
        {
            
            using (var stream = new MemoryStream())
            {
                Assert.Throws<SerializationException>(() => _serializer.Deserialize<JSonDataTest>(new StreamReader(stream)));
            }
        }
    }
}