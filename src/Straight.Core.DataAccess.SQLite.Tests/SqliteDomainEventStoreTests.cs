using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Straight.Core.DataAccess.Serialization;
using Straight.Core.EventStore;
using Straight.Core.Tests.Common;

namespace Straight.Core.DataAccess.SQLite.Tests
{
    [TestFixture]
    public class SqliteDomainEventStoreTests
    {
        private SqLiteDomainEventStore<IDomainEvent> driver;
        private string _dbName;
        
        [SetUp]
        public void Setup()
        {
            if (!AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Contains("Microsoft.Data.Sqlite")))
            {
                AppDomain.CurrentDomain.Load("Microsoft.Data.Sqlite");
                var instance = Microsoft.Data.Sqlite.SqliteFactory.Instance;
            }
            _dbName = "Data Source=:memory:";
            driver = new SqLiteDomainEventStore<IDomainEvent>(_dbName, new JSonEventSerializer());
        }

        [TearDown]
        public void TearDown()
        {
            driver.Dispose();
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
