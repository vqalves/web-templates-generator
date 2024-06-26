namespace WebCodeGenerator.Templates.SqlRepository.Payloads
{
    public class SqlRepositoryRequestPayloadSingleTable
    {
        public string? OutputNamespace { get; set; }
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? TableSchema { get; set; }
        public string? TableName { get; set; }
    }
}
