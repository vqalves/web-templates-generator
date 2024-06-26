namespace WebCodeGenerator.Infrastructure.SqlServer.ValueObjects
{
    public record SqlServerColumnType
    {
        public static readonly SqlServerColumnType Varchar = new(
            sqlName: "varchar",
            csharpName: "string",
            safeToStringSplit: false
        );

        public static readonly SqlServerColumnType UniqueIdentifier = new(
            sqlName: "uniqueidentifier",
            csharpName: "Guid",
            safeToStringSplit: true
        );

        public static readonly SqlServerColumnType Integer = new(
            sqlName: "int",
            csharpName: "int",
            safeToStringSplit: true
        );

        public string SqlName { get; }
        public string CsharpName { get; }
        public bool SafeToStringSplit { get; }

        public SqlServerColumnType(
            string sqlName, 
            string csharpName, 
            bool safeToStringSplit
        )
        {
            SqlName = sqlName;
            CsharpName = csharpName;
            SafeToStringSplit = safeToStringSplit;
        }

        public static IEnumerable<SqlServerColumnType> ListAll()
        {
            yield return Varchar;
            yield return UniqueIdentifier;
            yield return Integer;
        }

        public static SqlServerColumnType? GetBySqlName(string? sqlName)
        {
            if (string.IsNullOrWhiteSpace(sqlName))
                return null;

            sqlName = sqlName.ToLower();
            return ListAll().FirstOrDefault(x => x.SqlName == sqlName);
        }
    }
}