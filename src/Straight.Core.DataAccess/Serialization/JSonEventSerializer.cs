using Newtonsoft.Json;
using Straight.Core.Serialization;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Straight.Core.DataAccess.Serialization
{
    public class JSonEventSerializer : IEventSerializer
    {
        public void Serialize(TextWriter writer, object graph)
        {
            writer.WriteLine(JsonConvert.SerializeObject(graph, Formatting.None));
            writer.Flush();
        }

        public TEvent Deserialize<TEvent>(TextReader reader)
        {
            try
            {
                var data = reader.ReadLine();
                return JsonConvert.DeserializeObject<TEvent>(data);
            }
            catch (Exception ex)
            {
                throw new SerializationException(ex.Message, ex);
            }
        }
    }
}