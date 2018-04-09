﻿using NUnit.Framework;
using Straight.Core.DataAccess.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Straight.Core.DataAccess.Tests
{
    [TestFixture]
    public class JSonDomainEventSerializerTests
    {
       
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

        [Test]
        public void Should_get_data_when_deserialize_data()
        {
            var jsonActual = "{\"Content\":\"Flash Gordon\",\"Integer\":10000}";
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(jsonActual);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                var data = new JSonEventSerializer().Deserialize<JSonDataTest>(new StreamReader(stream));
                Assert.NotNull(data);
                Assert.That(data.Content, Is.EqualTo("Flash Gordon"));
                Assert.That(data.Integer, Is.EqualTo(10000));
            }
        }

        [Test]
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
                new JSonEventSerializer().Serialize(new StreamWriter(stream), data);
                stream.Position = 0;
                jsonActual = new StreamReader(stream).ReadToEnd();
            }
            Assert.NotNull(jsonActual);
            Assert.That(jsonActual, Is.Not.Empty);
            Assert.That(jsonActual, Is.EqualTo("{\"Content\":\"Flash Gordon\",\"Integer\":10000}" + Environment.NewLine));
        }

        [Test]
        public void Should_throw_deserialization_exception_when_null_reference()
        {

            using (var stream = new MemoryStream())
            {
                Assert.Throws<SerializationException>(() => new JSonEventSerializer().Deserialize<JSonDataTest>(new StreamReader(stream)));
            }
        }
    }
}