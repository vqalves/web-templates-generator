namespace WebCodeGenerator.Infrastructure.SqlServer.ValueObjects
{
    public class SqlServerIndexColumn
    {
        public int ObjectID { get; init; }
        public int IndexID { get; init; }
        public int IndexColumnID { get; init; }
        public int ColumnID { get; init; }
        public byte KeyOrdinal { get; init; }

        public SqlServerIndexColumn(int objectId, int indexId, int indexColumnId, int columnId, byte keyOrdinal)
        {
            ObjectID = objectId;
            IndexID = indexId;
            IndexColumnID = indexColumnId;
            ColumnID = columnId;
            KeyOrdinal = keyOrdinal;
        }
    }
}
