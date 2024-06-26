using System.Collections;
using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Infrastructure.SqlServer.Collections
{
    public class SqlServerTableCollection : IEnumerable<SqlServerTable>
    {
        private readonly IEnumerable<SqlServerTable> Items;

        public SqlServerTableCollection(IEnumerable<SqlServerTable> items)
        {
            Items = items;
        }

        public SqlServerTable? Find(string? schema, string? tableName)
        {
            if (string.IsNullOrWhiteSpace(schema))
                return null;

            if (string.IsNullOrWhiteSpace(tableName))
                return null;

            return Items
                .Where(x => string.Equals(x.Name, tableName, StringComparison.OrdinalIgnoreCase))
                .Where(x => string.Equals(x.Schema, schema, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
        }

        public IEnumerator<SqlServerTable> GetEnumerator() => Items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
    }
}
