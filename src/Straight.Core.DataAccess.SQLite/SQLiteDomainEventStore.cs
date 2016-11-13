using Microsoft.Data.Sqlite;
using Straight.Core.EventStore;
using Straight.Core.EventStore.Aggregate;
using Straight.Core.EventStore.Storage;
using Straight.Core.Extensions.Collections.Generic;
using Straight.Core.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using System.Linq;

namespace Straight.Core.DataAccess.SQLite
{
    public sealed class SqLiteDomainEventStore<TDomainEvent> : DomainEventStoreBase<TDomainEvent>
        where TDomainEvent : IDomainEvent
    {
        private const string TableName = @"{0}Events";

        private const string InsertEventFormat =
            @"INSERT INTO {0} VALUES(@EventId, @AggregatorId, @Event, @Version)";

        private const string SelectLastVersionEventFormat =
            @"SELECT ifnull(Max(Version), 0) From {0} Where AggregatorId=@AggregatorId";

        private const string SelectEventsFormat =
            @"SELECT EventId, AggregatorId, Event, Version From {0} Where AggregatorId=@AggregatorId";

        private static ImmutableDictionary<string, string> _typeToSelect = ImmutableDictionary<string, string>.Empty;
        private static ImmutableDictionary<string, string> _typeToInsert = ImmutableDictionary<string, string>.Empty;
        private static ImmutableDictionary<string, string> _typeToLastVersion = ImmutableDictionary<string, string>.Empty;
        private readonly string _connectionString;
        private readonly IEventSerializer _serializer;
        private SqliteConnection _sqliteConnection;
        private SqliteTransaction _sqLiteTransaction;
        private readonly string selectSql;
        private readonly string lastVersionSql;
        private readonly string insertSql;

        public SqLiteDomainEventStore(string connectionString, string tableName, IEventSerializer serializer)
        {
            _connectionString = connectionString;
            _serializer = serializer;
            selectSql = GetSelectCommandText(tableName);
            lastVersionSql = GetLastVersionCommandText(tableName);
            insertSql = GetInsertCommandText(tableName);
        }

        protected override void BeginTransactionOverride()
        {
            _sqliteConnection = GetConnection();
            _sqLiteTransaction = _sqliteConnection.BeginTransaction();
        }

        private SqliteConnection GetConnection()
        {
            var sqliteConnection = new SqliteConnection(_connectionString);
            sqliteConnection.Open();
            return sqliteConnection;
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
            using (var ms = SelectEvents(aggregateId))
            {
                var streamReader = new StreamReader(ms);
                while (!streamReader.EndOfStream)
                {
                    yield return _serializer.Deserialize<TDomainEvent>(streamReader);
                }
            }
        }

        private Stream SelectEvents(Guid aggregateId)
        {

            using (var connection = GetConnection())
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = selectSql;
                cmd.Parameters.Add(new SqliteParameter("@AggregatorId", aggregateId));
                return WriteEvents(cmd.ExecuteReader());
            }
        }

        private static Stream WriteEvents(IDataReader reader)
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            while (reader.Read())
            {
                writer.WriteLine(reader["Event"]);
            }
            writer.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        protected override void SaveOverride(IDomainEventChangeable<TDomainEvent> aggregator)
        {
            var commandText = insertSql;
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
            using (var sqLiteCommand = new SqliteCommand(lastVersionSql, _sqLiteTransaction.Connection, _sqLiteTransaction))
            {
                sqLiteCommand.Parameters.Add(new SqliteParameter("@AggregatorId", aggregator.Id));
                var executeScalar = sqLiteCommand.ExecuteScalar();
                return executeScalar == null ? 0 : Convert.ToInt32(executeScalar);
            }
        }



        private string Serialize(TDomainEvent @event)
        {
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(new StreamWriter(stream), @event);
                return new StreamReader(stream).ReadToEnd();
            }
        }

        private string GetSelectCommandText(string table)
        {
            string select;
            if (_typeToSelect.TryGetValue(table, out select))
            {
                return select;
            }
            var tableName = string.Format(TableName, table);
            select = string.Format(SelectEventsFormat, tableName);
            _typeToSelect = _typeToSelect.Add(table, select);
            CheckIfTableExist(tableName);
            return select;
        }

        private string GetLastVersionCommandText(string table)
        {
            string select;
            if (_typeToLastVersion.TryGetValue(table, out select))
            {
                return select;
            }
            var tableName = string.Format(TableName, table);
            select = string.Format(SelectLastVersionEventFormat, tableName);
            _typeToLastVersion = _typeToLastVersion.Add(table, select);
            CheckIfTableExist(tableName);
            return select;
        }

        private string GetInsertCommandText(string table)
        {
            string insert;
            if (_typeToInsert.TryGetValue(table, out insert))
            {
                return insert;
            }
            var tableName = string.Format(TableName, table);
            insert = string.Format(InsertEventFormat, tableName);
            _typeToInsert = _typeToInsert.Add(table, insert);
            CheckIfTableExist(table);
            return insert;
        }

        private void CheckIfTableExist(string tableName)
        {
            string createTableIfNotExist =
                $"create table if not exists {tableName} (AggregatorId TEXT, EventId Text, Event TEXT, Version INTEGER)";
            using (var sqLiteCommand = new SqliteCommand(createTableIfNotExist, _sqLiteTransaction.Connection,
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
    }
}