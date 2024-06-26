using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Templates.SqlRepository.Models
{
    public class SqlRepositoryModel_DeleteCommand
    {
        private readonly SqlServerTable Table;
        public IEnumerable<SqlServerColumn> Columns;

        public SqlRepositoryModel_DeleteCommand(SqlServerTable table, IEnumerable<SqlServerColumn> keyColumns) 
        {
            this.Table = table;
            this.Columns = keyColumns;
        }

        public string CreateDeleteCommand()
        {
            var parameters = string.Join(" AND ", Columns.Select(x => $"{x.Name} = {GetParameterName(x)}"));
            var command = $"DELETE FROM {Table.Schema}.{Table.Name} WHERE {parameters}";

            return command;
        }

        private string GetParameterName(SqlServerColumn column)
        {
            return $"@{column.Name}";
        }
    }
}
