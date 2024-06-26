using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using WebCodeGenerator.Infrastructure.SqlServer.Collections;
using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Infrastructure.SqlServer.Services
{
    public class SqlServerService
    {
        private readonly SqlServerConnectionString ConnectionString;

        public SqlServerService(SqlServerConnectionString connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqlServerTableCollection ListTables(string databaseName)
        {
            var result = new List<SqlServerTable>();

            using var connection = new SqlConnection(ConnectionString.Value);
            
            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT object_id, name, SCHEMA_NAME(schema_id) schema_name FROM sys.tables";

            connection.Open();
            connection.ChangeDatabase(databaseName);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var objectId = Convert.ToInt32(reader["object_id"]);

                var data = new SqlServerTable
                (
                    objectId: objectId,
                    schema: reader["schema_name"].ToString()!,
                    name: reader["name"].ToString()!
                );

                result.Add(data);
            }

            return new SqlServerTableCollection(result);
        }

        public SqlServerColumnCollection ListColumns(string databaseName, SqlServerTable table)
        {
            var result = new List<SqlServerColumn>();

            using var connection = new SqlConnection(ConnectionString.Value);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name, column_id, TYPE_NAME(system_type_id) type_name, is_identity FROM sys.columns WHERE object_id = @ObjectID";
            command.Parameters.AddWithValue("@ObjectID", table.ObjectID);

            connection.Open();
            connection.ChangeDatabase(databaseName);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var typeName = reader["type_name"].ToString();
                var type = SqlServerColumnType.GetBySqlName(typeName);

                if (type == null)
                    throw new ArgumentException($"Cannot convert column type '{typeName}'");

                var columnId = Convert.ToInt32(reader["column_id"]);

                var data = new SqlServerColumn
                (
                    objectId: table.ObjectID,
                    name: reader["name"].ToString()!,
                    type: type,
                    columnId: columnId,
                    isIdentity: Convert.ToBoolean(reader["is_identity"])
                );

                result.Add(data);
            }

            return new SqlServerColumnCollection(result);
        }

        public SqlServerIndexCollection ListIndexes(string databaseName, SqlServerTable table)
        {
            var result = new List<SqlServerIndex>();

            using var connection = new SqlConnection(ConnectionString.Value);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT name, index_id, is_primary_key FROM sys.indexes WHERE object_id = @ObjectID";
            command.Parameters.AddWithValue("@ObjectID", table.ObjectID);

            connection.Open();
            connection.ChangeDatabase(databaseName);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var indexId = Convert.ToInt32(reader["index_id"]);
                var isPrimaryKey = Convert.ToBoolean(reader["is_primary_key"]);

                var data = new SqlServerIndex
                (
                    objectId: table.ObjectID,
                    name: reader["name"].ToString()!,
                    indexId: indexId,
                    isPrimaryKey: isPrimaryKey
                );

                result.Add(data);
            }

            return new SqlServerIndexCollection(result);
        }

        public SqlServerIndexColumnCollection ListIndexColumns(string databaseName, SqlServerTable table)
        {
            var result = new List<SqlServerIndexColumn>();

            using var connection = new SqlConnection(ConnectionString.Value);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT index_id, index_column_id, column_id, key_ordinal FROM sys.index_columns WHERE object_id = @ObjectID";
            command.Parameters.AddWithValue("@ObjectID", table.ObjectID);

            connection.Open();
            connection.ChangeDatabase(databaseName);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var indexId = Convert.ToInt32(reader["index_id"]);
                var indexColumnId = Convert.ToInt32(reader["index_column_id"]);
                var columnId = Convert.ToInt32(reader["column_id"]);
                var keyOrdinal = Convert.ToByte(reader["key_ordinal"]);

                var data = new SqlServerIndexColumn
                (
                    objectId: table.ObjectID,
                    indexId: indexId,
                    indexColumnId: indexColumnId,
                    columnId: columnId,
                    keyOrdinal: keyOrdinal
                );

                result.Add(data);
            }

            return new SqlServerIndexColumnCollection(result);
        }

        public async Task<Model?> FindAsync(int? key)
        {
            if (key == null) return null;

            var query = FilterAsync(ids: [key.Value]);
            var result = await query.FirstOrDefaultAsync();
            return result;
        }

        public class Model
        {

        }

        public async IAsyncEnumerable<Model> FilterAsync(
            IEnumerable<int>? ids = null,
            ILogger logger = null
        )
        {
            // Setup
            var joins = new List<string>();
            var parameters = new Dictionary<string, object>();
            var filters = new List<string>();

            var select = "SELECT Table.columns FROM Table";

            if(ids != null)
            {
                joins.Add("INNER JOIN (SELECT CONVERT(INT, Value) Value FROM STRING_SPLIT(',', @IDs)) as FilterID ON Table.ID = FilterID.Value");
                parameters.Add("@IDs", string.Join(",", ids));
            }

            if(joins.Any())
                select += Environment.NewLine + string.Join(Environment.NewLine, joins);

            if (filters.Any())
                select += Environment.NewLine + "WHERE " + string.Join(" AND ", filters);

            // Execution
            using var connection = new SqlConnection(ConnectionString.Value);
            var queryResult = connection.QueryUnbufferedAsync<Model>(select, parameters);

            await foreach(var item in queryResult)
                yield return item;
        }
    }
}