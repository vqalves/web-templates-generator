using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO.Compression;
using WebCodeGenerator.Infrastructure.SqlServer.Services;
using WebCodeGenerator.Infrastructure.SqlServer.ValueObjects;
using WebCodeGenerator.Templates.SqlRepository.Models;
using WebCodeGenerator.Templates.SqlRepository.Payloads;

namespace WebCodeGenerator.Controllers
{
    public class TemplatesController : Controller
    {
        private readonly ILogger<TemplatesController> _logger;

        public TemplatesController(ILogger<TemplatesController> logger)
        {
            _logger = logger;
        }

        

        [HttpPost]
        [Route("/template/sql-repository/all-tables")]
        public IActionResult SqlRepositoryAllTables([FromBody] SqlRepositoryRequestPayloadAllTables payload)
        {
            if (!SqlServerConnectionString.TryParse(payload.ConnectionString, out var connectionString))
                throw new ArgumentException("ConnectionString is invalid");

            var sqlService = new SqlServerService(connectionString!);
            var tables = sqlService.ListTables(payload.DatabaseName!);

            var ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var table in tables)
                {
                    var columns = sqlService.ListColumns(payload.DatabaseName, table);
                    var indexes = sqlService.ListIndexes(payload.DatabaseName, table);
                    var indexColumns = sqlService.ListIndexColumns(payload.DatabaseName, table);

                    var model = new SqlRepositoryModel
                    (
                        table: table,
                        columns: columns,
                        indexColumns: indexColumns,
                        indexes: indexes,
                        outputNamespace: payload.OutputNamespace!
                    );

                    AddRepositoryFile(model, zip);
                    AddInterfaceFile(model, zip);
                }
            }

            ms.Position = 0;
            return File(ms, "application/zip");
        }

        

        [HttpPost]
        [Route("/template/sql-repository/single-table")]
        public IActionResult SqlRepository([FromBody] SqlRepositoryRequestPayloadSingleTable payload)
        {
            if (!SqlServerConnectionString.TryParse(payload.ConnectionString, out var connectionString))
                throw new ArgumentException("ConnectionString is invalid");

            var sqlService = new SqlServerService(connectionString!);
            var tables = sqlService.ListTables(payload.DatabaseName!);

            var table = tables.Find(payload.TableSchema, payload.TableName);
            if (table == null)
                throw new ArgumentException($"Could not find table '{payload.TableSchema}.{payload.TableName}'");

            var columns = sqlService.ListColumns(payload.DatabaseName, table);
            var indexes = sqlService.ListIndexes(payload.DatabaseName, table);
            var indexColumns = sqlService.ListIndexColumns(payload.DatabaseName, table);

            var model = new SqlRepositoryModel
            (
                table: table,
                columns: columns,
                indexColumns: indexColumns,
                indexes: indexes,
                outputNamespace: payload.OutputNamespace!
            );

            var ms = new MemoryStream();
            using (ZipArchive zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                AddRepositoryFile(model, zip);
                AddInterfaceFile(model, zip);
            }

            ms.Position = 0;
            return File(ms, "application/zip");
        }

        private void AddRepositoryFile(
            SqlRepositoryModel model,
            ZipArchive zipArchive)
        {
            var document = RenderRazorViewToString(this, "/Templates/SqlRepository/RepositoryTemplate.cshtml", model);
            var fileName = $"{model.GetRepositoryClassName()}.cs";

            var entry = zipArchive.CreateEntry($"Repositories/{fileName}", CompressionLevel.Optimal);

            using var entryStream = entry.Open();
            using (var writer = new StreamWriter(entryStream))
            {
                writer.Write(document);
                writer.Flush();
            }
        }

        private void AddInterfaceFile(
            SqlRepositoryModel model,
            ZipArchive zipArchive)
        {
            var document = RenderRazorViewToString(this, "/Templates/SqlRepository/InterfaceTemplate.cshtml", model);
            var fileName = $"{model.GetRepositoryInterfaceName()}.cs";

            var entry = zipArchive.CreateEntry($"Interfaces/{fileName}", CompressionLevel.Optimal);

            using var entryStream = entry.Open();
            using (var writer = new StreamWriter(entryStream))
            {
                writer.Write(document);
                writer.Flush();
            }
        }

        private string RenderRazorViewToString(Controller controller, string viewPath, object model = null)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                IViewEngine viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                
                ViewEngineResult viewResult = viewEngine.GetView("", viewPath, false);

                ViewContext viewContext = new ViewContext
                (
                    controller.ControllerContext,
                    viewResult.View,
                    controller.ViewData,
                    controller.TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
