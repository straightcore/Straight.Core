using System.Data;
using System.Reflection;
using Microsoft.Data.Sqlite;
using Straight.Core.Common.Runtime;
using Straight.Core.DataAccess.Data;
using Straight.Core.DataAccess.Serialization;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common;
using Xunit;

namespace Straight.Core.DataAccess.SqlLite.Tests
{
    public class SqliteDomainEventStoreTests
    {
        public SqliteDomainEventStoreTests()
        {
            var assembly = "Microsoft.Data.Sqlite".GetAssembly();
            if (assembly == null)
            {
                Assembly.Load(new AssemblyName("Microsoft.Data.Sqlite"));
                var instance = SqliteFactory.Instance;
            }
            
            const string tableName = "XUNIT_TEST";
            _driver = new SqLiteDomainEventStore<IDomainEvent>(new TestSQLiteConnectionFactory(tableName), tableName, new JSonEventSerializer());
        }

        private readonly SqLiteDomainEventStore<IDomainEvent> _driver;
        
        [Fact]
        public void Should_not_save_when_rollback_transaction()
        {
            _driver.BeginTransaction();
            _driver.Save(PersonaAggregator.CreateNewAggregatorTest(() => { }));
            _driver.Rollback();
        }

        [Fact]
        public void Should_save_new_aggregate_when_commit()
        {
            _driver.BeginTransaction();
            _driver.Save(PersonaAggregator.CreateNewAggregatorTest(() => { }));
            _driver.Commit();
        }

       
    }

    public class TestSQLiteConnectionFactory : ISqlConnectionFactory<SqliteConnection>
    {
        private readonly string _tableName;
        public string ConnectionString => "Data Source=:memory:";

        public TestSQLiteConnectionFactory(string tableName)
        {
            _tableName = tableName;
        }

        public SqliteConnection OpenConnection()
        {
            var connectionMemory = new SqliteConnection(ConnectionString);
            connectionMemory.Open();
            LoadInMemoryTable(connectionMemory, _tableName);
            return connectionMemory;
        }

        private static void LoadInMemoryTable(SqliteConnection cnx, string tableName)
        {
            var createTableIfNotExist = $"create table if not exists {tableName} (AggregatorId TEXT, EventId Text, Event TEXT, Version INTEGER)";
            using (var sqLiteTranasction = cnx.BeginTransaction(IsolationLevel.Serializable))
            using (var sqLiteCommand = new SqliteCommand(createTableIfNotExist, cnx, sqLiteTranasction))
            {
                var result = sqLiteCommand.ExecuteScalar();
                sqLiteTranasction.Commit();
            }
        }
    }
}
