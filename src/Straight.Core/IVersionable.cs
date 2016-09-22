
namespace Straight.Core
{
    public interface IVersionable
    {
        int Version { get; }
    }

    public interface IVersionableUpdatable : IVersionable
    {
        void UpdateVersion(int version);
    }
}
