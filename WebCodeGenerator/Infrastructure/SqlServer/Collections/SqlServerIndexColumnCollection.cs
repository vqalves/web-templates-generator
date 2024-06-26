using System.Collections;
using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Infrastructure.SqlServer.Collections
{
    public class SqlServerIndexColumnCollection : IEnumerable<SqlServerIndexColumn>
    {
        private readonly List<SqlServerIndexColumn> IndexColumns;

        public SqlServerIndexColumnCollection(List<SqlServerIndexColumn> indexColumns)
        {
            IndexColumns = indexColumns;
        }

        public IEnumerable<SqlServerIndexColumn> List(SqlServerIndex index)
        {
            return IndexColumns
                .Where(x => x.ObjectID == index.ObjectID)
                .Where(x => x.IndexID == index.IndexID);
        }

        public IEnumerator<SqlServerIndexColumn> GetEnumerator() => IndexColumns.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => IndexColumns.GetEnumerator();
    }
}