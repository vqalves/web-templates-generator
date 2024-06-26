using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;

namespace WebCodeGenerator.Templates.SqlRepository.Models
{
    public class SqlRepositoryModel_FilterQuery
    {
        public SqlRepositoryModel Model { get; init; }
        public SqlServerColumn? SingleKeyColumn { get; init; }

        public SqlRepositoryModel_FilterQuery(
            SqlServerColumn? singleKeyColumn, 
            SqlRepositoryModel model)
        {
            SingleKeyColumn = singleKeyColumn;
            Model = model;
        }

        public bool HasKey()
        {
            return SingleKeyColumn != null;
        }

        public string GetMethodSignature()
        {
            if(SingleKeyColumn != null)
            {
                return $@"IAsyncEnumerable<{Model.GetModelName()}> FilterAsync(IEnumerable<{SingleKeyColumn.Type.CsharpName}>? ids = null)";
            }
            else
            {
                return $@"IAsyncEnumerable<{Model.GetModelName()}> FilterAsync()";
            }
        }
    }
}
