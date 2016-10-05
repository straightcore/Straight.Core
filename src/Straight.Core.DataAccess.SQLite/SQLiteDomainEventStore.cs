using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.EventStore.Storage;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Serialization;

namespace Straight.Core.DataAccess.SQLite
{
    public sealed class SqLiteDomainEventStore<TDomainEvent> : DomainEventStoreBase<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private const string TableName = @"{0}Events";

        private const string InsertEventFormat =
            @"INSERT INTO {0} VALUES(@EventId, @AggregatorId, @Event, @Version)";

        private const string SelectLastVersionEventFormat =
            @"SELECT Max(Version) From {0} Where AggregatorId=@AggregatorId";

        private static ImmutableDictionary<Type, string> TypeToInsert = ImmutableDictionary<Type, string>.Empty;
        private readonly string _connectionString;
        private readonly IEventSerializer _serializer;
        private SqliteConnection _sqliteConnection;
        private SqliteTransaction _sqLiteTransaction;

        public SqLiteDomainEventStore(string connectionString, IEventSerializer serializer)
        {
            _connectionString = connectionString;
            _serializer = serializer;
        }

        protected override void BeginTransactionOverride()
        {
            _sqliteConnection = new SqliteConnection(_connectionString);
            _sqliteConnection.Open();
            _sqLiteTransaction = _sqliteConnection.BeginTransaction();
        }

        protected override void CommitOverride()
        {
            _sqLiteTransaction.Commit();
            CloseConnection();
        }

        protected override void RollbackOverride()
        {
            _sqLiteTransaction.Rollback();
            CloseConnection();
        }

        private void CloseConnection()
        {
            _sqLiteTransaction.Dispose();
            _sqliteConnection.Close();
            _sqliteConnection.Dispose();
            _sqLiteTransaction = null;
            _sqliteConnection = null;
        }

        protected override IEnumerable<TDomainEvent> GetOverride(Guid aggregateId)
        {
            throw new NotImplementedException();
        }

        protected override void SaveOverride(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            var commandText = GetCommandText(aggregator.GetType());
            aggregator.GetChanges().ForEach(ev => SaveEvent(ev, commandText, _sqLiteTransaction));
        }

        private void SaveEvent(TDomainEvent @event, string commandText, SqliteTransaction sqLiteTransaction)
        {
            using (var sqLiteCommand = new SqliteCommand(commandText, sqLiteTransaction.Connection, sqLiteTransaction))
            {
                sqLiteCommand.Parameters.Add(new SqliteParameter("@EventId", @event.Id));
                sqLiteCommand.Parameters.Add(new SqliteParameter("@AggregatorId", @event.AggregateId));
                sqLiteCommand.Parameters.Add(new SqliteParameter("@Event", Serialize(@event)));
                sqLiteCommand.Parameters.Add(new SqliteParameter("@Version", @event.Version));

                sqLiteCommand.ExecuteNonQuery();
            }
        }

        protected override int GetVersionAggregator(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            const string commandText = @"
                INSERT OR IGNORE INTO EventProviders VALUES (@eventProviderId, @type, 0);
                SELECT Version FROM EventProviders WHERE EventProviderId = @eventProviderId";
            using (var sqLiteCommand = new SqliteCommand(commandText, _sqLiteTransaction.Connection, _sqLiteTransaction)
            )
            {
                sqLiteCommand.Parameters.Add(new SqliteParameter("@AggregatorId", aggregator.Id));
                sqLiteCommand.Parameters.Add(new SqliteParameter("@type", aggregator.GetType().FullName));
                sqLiteCommand.Parameters.Add(new SqliteParameter("@version", aggregator.Version));

                var executeScalar = sqLiteCommand.ExecuteScalar();
                return executeScalar == null ? 0 : Convert.ToInt32(executeScalar);
            }
        }

        private string Serialize(TDomainEvent @event)
        {
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(new StreamWriter(stream), @event);
                return (new StreamReader(stream)).ReadToEnd();
            }
        }

        private string GetCommandText(Type type)
        {
            string insert;
            if (TypeToInsert.TryGetValue(type, out insert))
            {
                return insert;
            }
            var tableName = string.Format(TableName, type.Name);
            insert = string.Format(InsertEventFormat, tableName);
            TypeToInsert = TypeToInsert.Add(type, insert);
            CheckIfTableExist(tableName);
            return insert;
        }

        private void CheckIfTableExist(string tableName)
        {
            string createTableIfNotExist =
                $"create table if not exists {tableName} (AggregatorId TEXT, EventId Text, Event TEXT, Version INTEGER)";
            using (
                var sqLiteCommand = new SqliteCommand(createTableIfNotExist, _sqLiteTransaction.Connection,
                    _sqLiteTransaction))
            {
                sqLiteCommand.ExecuteScalar();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Rollback();
            }
        }


        private struct DBQuery
        {
            public string Select;
            public string LastVersion;
        }
    }
}