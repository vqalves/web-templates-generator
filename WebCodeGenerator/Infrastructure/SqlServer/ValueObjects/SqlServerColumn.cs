namespace WebCodeGenerator.Infrastructure.SqlServer.ValueObjects
{
    public class SqlServerColumn
    {
        public int ObjectID { get; init; }
        public int ColumnID { get; init; }
        public string Name { get; init; }
        public SqlServerColumnType Type { get; init; }
        public bool IsIdentity { get; init; }

        public SqlServerColumn(string name, SqlServerColumnType type, int objectId, int columnId, bool isIdentity)
        {
            Name = name;
            Type = type;
            ObjectID = objectId;
            ColumnID = columnId;
            IsIdentity = isIdentity;
        }

        public string FormatAsCsharpParameter()
        {
            return $"{Type.CsharpName} {Name}";
        }
    }
}