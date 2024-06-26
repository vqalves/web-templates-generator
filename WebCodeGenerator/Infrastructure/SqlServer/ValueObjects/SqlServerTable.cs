namespace WebCodeGenerator.Infrastructure.SqlServer.ValueObjects
{
    public class SqlServerTable
    {
        public int ObjectID { get; init; }
        public string Schema { get; init; }
        public string Name { get; init; }

        public SqlServerTable(string schema, string name, int objectId)
        {
            Schema = schema;
            Name = name;
            ObjectID = objectId;
        }
    }
}
