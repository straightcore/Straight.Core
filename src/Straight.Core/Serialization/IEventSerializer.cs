using System.IO;
using Straight.Core.EventStore;

namespace Straight.Core.Serialization
{
    public interface IEventSerializer
    {
        void Serialize(TextWriter writer, object graph);

        TEvent Deserialize<TEvent>(TextReader reader);
    }
}