using Microsoft.Data.Sqlite;
using Straight.Core.DataAccess.Data;
using System.Data;

namespace Straight.Core.DataAccess.SQLite
{
    public class SQLiteFactory : ISqlFactory //ISqlConnectionFactory<SQLiteConnection>
    {
        public string ConnectionString { get; }
        
        public SQLiteFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public IDbConnection CreateOpenConnection()
        {
            var cnx = SqliteFactory.Instance.CreateConnection();
            cnx.ConnectionString = ConnectionString;
            cnx.Open();
            return cnx;
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction)
        {
            return new SqliteCommand(commandText, connection as SqliteConnection, transaction as SqliteTransaction);
        }

        public IDbCommand CreateCommand(string commandText)
        {
            return new SqliteCommand(commandText, (SqliteConnection)CreateOpenConnection());
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection connection)
        {
            return new SqliteCommand(commandText, connection as SqliteConnection);
        }

    }
}
