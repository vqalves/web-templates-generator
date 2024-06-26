using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Templates.SqlRepository.Models
{
    public class SqlRepositoryModel_InsertCommand
    {
        private readonly SqlServerTable Table;
        public IEnumerable<SqlServerColumn> Columns;

        public SqlRepositoryModel_InsertCommand(SqlServerTable table, IEnumerable<SqlServerColumn> keyColumns) 
        {
            this.Table = table;
            this.Columns = keyColumns;
        }

        public string CreateInsertCommand()
        {
            var columns = string.Join(", ", Columns.Select(x => x.Name));
            var parameters = string.Join(", ", Columns.Select(x => GetParameterName(x)));

            var command = $"INSERT INTO {Table.Schema}.{Table.Name} ({columns}) VALUES ({parameters})";
            return command;
        }

        private string GetParameterName(SqlServerColumn column)
        {
            return $"@{column.Name}";
        }
    }
}
