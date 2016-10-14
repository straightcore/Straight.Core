using Microsoft.Data.Sqlite;
using NUnit.Framework;
using Straight.Core.DataAccess.Serialization;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common;
using System;
using System.Linq;

namespace Straight.Core.DataAccess.SQLite.Tests
{
    [TestFixture]
    public class SqliteDomainEventStoreTests
    {
        [SetUp]
        public void Setup()
        {
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Contains("Microsoft.Data.Sqlite")))
            {
                AppDomain.CurrentDomain.Load("Microsoft.Data.Sqlite");
                var instance = SqliteFactory.Instance;
            }
            _dbName = "Data Source=:memory:";
            driver = new SqLiteDomainEventStore<IDomainEvent>(_dbName, new JSonEventSerializer());
        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
        }

        private SqLiteDomainEventStore<IDomainEvent> driver;
        private string _dbName;

        [Test]
        public void Should_not_save_when_rollback_transaction()
        {
            driver.BeginTransaction();
            driver.Save(PersonaAggregator.CreateNewAggregatorTest(() => { }));
            driver.Rollback();
        }

        [Test]
        public void Should_save_new_aggregate_when_commit()
        {
            driver.BeginTransaction();
            driver.Save(PersonaAggregator.CreateNewAggregatorTest(() => { }));
            driver.Commit();
        }
    }
}