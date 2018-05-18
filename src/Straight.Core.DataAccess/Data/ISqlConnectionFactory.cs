using System;
using System.Data;

namespace Straight.Core.DataAccess.Data
{
    public interface ISqlFactory
    {
        string ConnectionString { get; }

        IDbConnection CreateOpenConnection();

        IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction);

        IDbCommand CreateCommand(string commandText);

        IDbCommand CreateCommand(string commandText, IDbConnection connection);
    }
}
