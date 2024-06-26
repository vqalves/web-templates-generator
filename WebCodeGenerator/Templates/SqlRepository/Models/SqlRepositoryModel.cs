using WebCodeGenerator.Infrastructure.SqlServer.Collections;
using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Templates.SqlRepository.Models
{
    public class SqlRepositoryModel
    {
        private Lazy<SqlRepositoryModel_PrimaryKey> _primaryKeyData;

        public string OutputNamespace { get; set; }
        public SqlServerTable Table { get; set; }
        public SqlServerColumnCollection Columns { get; set; }
        public SqlServerIndexCollection Indexes { get; set; }
        public SqlServerIndexColumnCollection IndexColumns { get; set; }

        public SqlRepositoryModel(
            SqlServerTable table,
            SqlServerColumnCollection columns,
            SqlServerIndexCollection indexes,
            SqlServerIndexColumnCollection indexColumns,
            string outputNamespace)
        {
            Table = table;
            Columns = columns;
            Indexes = indexes;
            IndexColumns = indexColumns;
            OutputNamespace = outputNamespace;

            _primaryKeyData = new Lazy<SqlRepositoryModel_PrimaryKey>(() =>
            {
                IEnumerable<SqlServerColumn> columns = [];

                var pk = Indexes.GetPrimaryKey(Table);

                if (pk != null)
                {
                    columns = IndexColumns
                        .List(pk)
                        .OrderBy(x => x.KeyOrdinal)
                        .Select(x => Columns.Find(x)!)
                        .ToList();
                }

                return new SqlRepositoryModel_PrimaryKey(columns);
            });
        }

        public string GetRepositoryClassName()
        {
            return $"{Table.Name}Repository";
        }

        public string GetRepositoryInterfaceName()
        {
            return $"I{Table.Name}Repository";
        }

        public string GetModelName()
        {
            return Table.Name;
        }

        public SqlRepositoryModel_InsertCommand CreateInsertCommand()
        {
            var nonIdentityColumns = Columns.Where(x => !x.IsIdentity);
            return new SqlRepositoryModel_InsertCommand(Table, nonIdentityColumns);
        }

        public SqlRepositoryModel_FilterQuery CreateFilterQuery()
        {
            SqlServerColumn? singleKeyColumn = null;

            var keyColumn = GetPrimaryKeyData();

            if (keyColumn.IsSingleColumn())
                singleKeyColumn = keyColumn.Columns.Single();

            return new SqlRepositoryModel_FilterQuery(
                singleKeyColumn: singleKeyColumn,
                model: this
            );
        }

        public string CreateUpdateCommand()
        {
            IEnumerable<SqlServerColumn> primaryColumns = SqlServerColumnCollection.Empty;
            IEnumerable<SqlServerColumn> nonPrimaryColumns = Columns;

            var primaryKeyData = GetPrimaryKeyData();

            if (HasPrimaryKey())
            {
                primaryColumns = GetPrimaryKeyData()!.Columns;
                nonPrimaryColumns = Columns.Where(x => !primaryColumns.Contains(x));
            }

            var nonPrimaryColumnsParameter = string.Join(", ", nonPrimaryColumns.Select(x => $"{x.Name} = {GetParameterName(x)}"));
            var command = $"UPDATE {Table.Schema}.{Table.Name} SET {nonPrimaryColumnsParameter}";

            if (primaryColumns.Any())
            {
                var primaryColumnsParameter = string.Join(" AND ", primaryColumns.Select(x => $"{x.Name} = {GetParameterName(x)}"));
                command += $" WHERE {primaryColumnsParameter}";
            }

            return command;
        }

        public SqlRepositoryModel_DeleteCommand CreateDeleteCommand()
        {
            IEnumerable<SqlServerColumn> keyColumns = Columns;

            if (HasPrimaryKey())
                keyColumns = GetPrimaryKeyData()!.Columns;

            return new SqlRepositoryModel_DeleteCommand(Table, keyColumns);
        }

        private string GetParameterName(SqlServerColumn column)
        {
            return $"@{column.Name}";
        }

        public SqlRepositoryModel_PrimaryKey GetPrimaryKeyData()
        {
            return _primaryKeyData.Value;
        }

        public bool HasPrimaryKey()
        {
            return GetPrimaryKeyData() != null;
        }
    }
}