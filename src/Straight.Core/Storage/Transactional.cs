namespace Straight.Core.Storage
{
    public interface Transactional
    {
        void BeginTransaction();
        void Commit();
        void Rollback();
    }
}
