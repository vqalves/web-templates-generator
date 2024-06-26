using System.Collections;
using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Infrastructure.SqlServer.Collections
{
    public class SqlServerColumnCollection : IEnumerable<SqlServerColumn>
    {
        public static readonly SqlServerColumnCollection Empty = new SqlServerColumnCollection([]);

        private readonly List<SqlServerColumn> Columns;

        public SqlServerColumnCollection(List<SqlServerColumn> columns)
        {
            Columns = columns;
        }

        public SqlServerColumn? Find(SqlServerIndexColumn indexColumn)
        {
            return Columns
                .Where(x => x.ObjectID == indexColumn.ObjectID)
                .Where(x => x.ColumnID == indexColumn.ColumnID)
                .FirstOrDefault();
        }

        public IEnumerator<SqlServerColumn> GetEnumerator() => Columns.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Columns.GetEnumerator();
    }
}