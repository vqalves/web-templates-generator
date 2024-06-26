
namespace WebCodeGenerator.Infrastructure.SqlServer.ValueObjects
{
    public record SqlServerConnectionString
    {
        public string Value { get; init; }

        public SqlServerConnectionString(string value)
        {
            Value = value;
        }

        public static bool TryParse(string? connectionString, out SqlServerConnectionString? result)
        {
            if(!string.IsNullOrWhiteSpace(connectionString)) 
            {
                result = new SqlServerConnectionString(connectionString!);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}