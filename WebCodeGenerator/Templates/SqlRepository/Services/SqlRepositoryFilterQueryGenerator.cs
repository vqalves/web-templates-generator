using WebCodeGenerator.Templates.SqlRepository.Models;

namespace WebCodeGenerator.Templates.SqlRepository.Services
{
    public class SqlRepositoryFilterQueryGenerator
    {
        public static string Generate(SqlRepositoryModel model)
        {
            var primaryKeyData = model.GetPrimaryKeyData();
            var columns = model.Columns.Select(x => $"{model.Table.Name}.{x.Name}");

            if (primaryKeyData.IsSingleColumn())
            {
                var keyColumn = primaryKeyData.Columns.First();
                
                var output = $@"    public async IAsyncEnumerable<{model.GetModelName()}> FilterAsync(
        IEnumerable<{keyColumn.Type.CsharpName}>? ids = null
    )
    {{
        // Setup
        var joins = new List<string>();
        var parameters = new Dictionary<string, object>();
        var filters = new List<string>();

        if(ids != null)
        {{
            filters.Add(""{keyColumn.Name} IN @IDs"");
            parameters.Add(""@IDs"", ids);
        }}

        // Command preparations
        var select = ""SELECT {string.Join(", ", columns)} FROM {model.Table.Schema}.{model.Table.Name}"";

        if(joins.Any())
            select += Environment.NewLine + string.Join(Environment.NewLine, joins);

        if (filters.Any())
            select += Environment.NewLine + ""WHERE "" + string.Join("" AND "", filters);

        // Execution
        using var connection = new SqlConnection(ConnectionString.Value);
        var queryResult = connection.QueryUnbufferedAsync<{model.GetModelName()}>(select, parameters);

        await foreach(var item in queryResult)
            yield return item;
    }}";

                return output;
            }
            else
            {
                var output = $@"    public async IAsyncEnumerable<{model.GetModelName()}> FilterAsync()
    {{
        // Setup
        var joins = new List<string>();
        var parameters = new Dictionary<string, object>();
        var filters = new List<string>();

        // Command preparation
        var select = ""SELECT {string.Join(", ", columns)} FROM {model.Table.Schema}.{model.Table.Name}"";

        if(joins.Any())
            select += Environment.NewLine + string.Join(Environment.NewLine, joins);

        if (filters.Any())
            select += Environment.NewLine + ""WHERE "" + string.Join("" AND "", filters);

        // Execution
        using var connection = new SqlConnection(ConnectionString.Value);
        var queryResult = connection.QueryUnbufferedAsync<{model.GetModelName()}>(select, parameters);

        await foreach(var item in queryResult)
            yield return item;
    }}";

                return output;
            }   
        }
    }
}
