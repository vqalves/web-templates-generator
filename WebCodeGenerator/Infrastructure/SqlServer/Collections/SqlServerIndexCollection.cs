using System.Collections;
using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Infrastructure.SqlServer.Collections
{
    public class SqlServerIndexCollection : IEnumerable<SqlServerIndex>
    {
        private readonly IEnumerable<SqlServerIndex> Indexes;

        public SqlServerIndexCollection(IEnumerable<SqlServerIndex> indexes)
        {
            Indexes = indexes;
        }

        public SqlServerIndex? GetPrimaryKey(SqlServerTable table)
        {
            return Indexes
                .Where(x => x.ObjectID == table.ObjectID)
                .Where(x => x.IsPrimaryKey)
                .FirstOrDefault();
        }


        public IEnumerator<SqlServerIndex> GetEnumerator() => Indexes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Indexes.GetEnumerator();
    }
}
