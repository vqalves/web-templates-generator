namespace WebCodeGenerator.Infrastructure.SqlServer.ValueObjects
{
    public class SqlServerIndex
    {
        public int ObjectID { get; init; }
        public string Name { get; init; }
        public int IndexID { get; init; }
        public bool IsPrimaryKey { get; init; }

        public SqlServerIndex(int objectId, string name, int indexId, bool isPrimaryKey)
        {
            ObjectID = objectId;
            Name = name;
            IndexID = indexId;
            IsPrimaryKey = isPrimaryKey;
        }
    }
}
