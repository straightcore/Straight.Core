using System.Data;
using System.Data.Common;

namespace Straight.Core.DataAccess.Data
{
    public interface ISqlConnectionFactory<out T>
         where T : IDbConnection
    {
        string ConnectionString { get; }

        T OpenConnection();


    }
}
