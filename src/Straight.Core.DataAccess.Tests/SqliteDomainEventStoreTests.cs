using System.Data;
using System.Reflection;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Straight.Core.DataAccess.Data;
using Straight.Core.DataAccess.Serialization;
using Straight.Core.DataAccess.SqlLite;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common;

namespace Straight.Core.DataAccess.Tests
{
    [TestFixture]
    public class SqliteDomainEventStoreTests
    {
        
        private const string tableName = "UNIT_TEST";

        private class DriverContext
        {
            public SqLiteDomainEventStore<IDomainEvent> Driver { get; } = new SqLiteDomainEventStore<IDomainEvent>(new TestSQLiteConnectionFactory(tableName), tableName, new JSonEventSerializer());
        }
        
        [Test]
        public void Should_not_save_when_rollback_transaction()
        { 
            var context = new DriverContext();
            context.Driver.BeginTransaction();
            context.Driver.Save(PersonaAggregator.CreateNewAggregatorTest(() => { }));
            context.Driver.Rollback();
        }

        [Test]
        public void Should_save_new_aggregate_when_commit()
        {
            var context = new DriverContext();
            context.Driver.BeginTransaction();
            context.Driver.Save(PersonaAggregator.CreateNewAggregatorTest(() => { }));
            context.Driver.Commit();
        }

       
    }

    public class TestSQLiteConnectionFactory : ISqlFactory
    {
        static TestSQLiteConnectionFactory()
        {
            var assembly = "Microsoft.Data.Sqlite".GetAssembly();
            if (assembly == null)
            {
                Assembly.Load(new AssemblyName("Microsoft.Data.Sqlite"));
                factory = SqliteFactory.Instance;
            }
        }
        private readonly static SqliteFactory factory;

        private readonly string _tableName;
        public string ConnectionString => "Data Source=:memory:";
        private readonly SqliteConnection _connection;

        public TestSQLiteConnectionFactory(string tableName)
        {
            _tableName = tableName;
            _connection = (SqliteConnection)factory.CreateConnection();
            _connection.ConnectionString = ConnectionString;
            _connection.Open();
            LoadInMemoryTable(_connection, _tableName);
        }

        private static void LoadInMemoryTable(SqliteConnection cnx, string tableName)
        {
            var createTableIfNotExist = $"create table if not exists {tableName}Events (AggregatorId TEXT, EventId Text, Event TEXT, Version INTEGER)";
            using (var SqliteTranasction = cnx.BeginTransaction(IsolationLevel.Serializable))
            using (var SqliteCommand = new SqliteCommand(createTableIfNotExist, cnx, SqliteTranasction))
            {
                var result = SqliteCommand.ExecuteScalar();
                SqliteTranasction.Commit();
            }
        }

        public IDbConnection CreateOpenConnection()
        {
            return _connection;
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection connection, IDbTransaction transaction)
        {
            return new SqliteCommand(commandText, connection as SqliteConnection, transaction as SqliteTransaction);
        }

        public IDbCommand CreateCommand(string commandText)
        {
            return new SqliteCommand(commandText, (SqliteConnection)CreateOpenConnection());
        }

        public IDbCommand CreateCommand(string commandText, IDbConnection dbConnection)
        {
            return new SqliteCommand(commandText, dbConnection as SqliteConnection);
        }
    }
}
