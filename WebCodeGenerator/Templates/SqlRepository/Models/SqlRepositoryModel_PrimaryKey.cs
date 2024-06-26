using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Templates.SqlRepository.Models
{
    public class SqlRepositoryModel_PrimaryKey
    {
        public IEnumerable<SqlServerColumn> Columns { get; init; }

        public SqlRepositoryModel_PrimaryKey(IEnumerable<SqlServerColumn> columns)
        {
            Columns = columns;
        }

        public bool HasPrimaryKey()
        {
            return Columns.Any();
        }

        public bool IsSingleColumn()
        {
            return Columns.Count() == 1;
        }

        public string FormatCsharpParameter()
        {
            var parameterNames = Columns.Select(x => x.FormatAsCsharpParameter());
            return string.Join(", ", parameterNames);
        }
    }
}
