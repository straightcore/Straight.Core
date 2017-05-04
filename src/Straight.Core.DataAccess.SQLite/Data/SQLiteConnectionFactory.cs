using System.Data;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Straight.Core.DataAccess.Data;

namespace Straight.Core.DataAccess.SQlLite.Data
{
    public class SQLiteConnectionFactory : ISqlConnectionFactory<SqliteConnection>
    {
        public string ConnectionString { get; }
        
        public SQLiteConnectionFactory(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqliteConnection OpenConnection()
        {
            var cnx = new SqliteConnection(ConnectionString);
            cnx.Open();
            return cnx;
        }
    }
}
