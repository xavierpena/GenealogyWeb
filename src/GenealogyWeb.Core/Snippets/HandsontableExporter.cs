using GenealogyWeb.Core.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GenealogyWeb.Core.Snippets
{
    /// <summary>
    /// Info:
    /// http://docs.handsontable.com/0.17.0/tutorial-quick-start.html#page-install
    /// </summary>
    public class HandsontableExporter
    {
        private const string BaseExportPath = @"C:\Users\xavier.pena\Documents\e-dric\Projects\github-tests\TestsGenealogia\data\handsondata";
        private const string HandsontableTemplatePath = @".\Resources\HandsontableHtml.txt";

        private PersonaRepository _personaRepository;
        private MatrimoniRepository _matrimoniRepository;
        private FillRepository _fillRepository;

        public HandsontableExporter(string connStr)
        {
            _personaRepository = new PersonaRepository(connStr);
            _matrimoniRepository = new MatrimoniRepository(connStr);
            _fillRepository = new FillRepository(connStr);
        }

        public void ExportResults()
        {
            ExportResultsWithContents();
            //ExportResultsWithoutContents();
        }

        private void ExportResultsWithContents()
        {
            var emptyRows = false;

            File.WriteAllText(
                path: Path.Combine(BaseExportPath, "fills.htm"),
                contents: GetFillsResults("Fills", emptyRows)
            );

            File.WriteAllText(
                path: Path.Combine(BaseExportPath, "matrimonis.htm"),
                contents: GetMatrimonisResults("Matrimonis", emptyRows)
            );

            File.WriteAllText(
                path: Path.Combine(BaseExportPath, "persones.htm"),
                contents: GetPersonesResults("Persones", emptyRows)
            );
        }

        private void ExportResultsWithoutContents()
        {
            var emptyRows = true;

            File.WriteAllText(
                path: Path.Combine(BaseExportPath, "fills-editar.htm"),
                contents: GetFillsResults("Fills - Editar", emptyRows)
            );

            File.WriteAllText(
                path: Path.Combine(BaseExportPath, "matrimonis-editar.htm"),
                contents: GetMatrimonisResults("Matrimonis - Editar", emptyRows)
            );

            File.WriteAllText(
                path: Path.Combine(BaseExportPath, "persones-editar.htm"),
                contents: GetPersonesResults("Persones - Editar", emptyRows)
            );
        }

        private string GetAsHtml(string title, string header, IEnumerable<string> rows)
        {
            var html = File.ReadAllText(HandsontableTemplatePath);

            var data = default(string);
            if (rows == null)
                data = "[" + header + "]";
            else
                data = "[" + string.Join(",", rows) + "]";

            return html
                .Replace("[[[title]]]", title)
                .Replace("[[[header]]]", title)
                .Replace("[[[colHeaders]]]", header)
                .Replace("[[[data]]]", data);
        }

        private string GetFillsResults(string title, bool emptyRows)
        {            
            var header = $"[\"id\",\"matrimoni_id\",\"persona_id\",\"observacions\"]";            
            var rows = default(IEnumerable<string>);
            if(!emptyRows)
            {
                var fills = _fillRepository.GetAll();
                rows = fills.Select(x => $"[\"{x.id}\",\"{x.matrimoni_id}\",\"{x.persona_id}\",\"{x.observacions}\"]");
            }
            return GetAsHtml(title, header, rows);
        }

        private string GetMatrimonisResults(string title, bool emptyRows)
        {            
            var header = $"[\"id\",\"home_id\",\"dona_id\",\"lloc\",\"data\",\"observacions\"]";
            var rows = default(IEnumerable<string>);
            if (!emptyRows)
            {
                var matrimonis = _matrimoniRepository.GetAll();
                rows = matrimonis.Select(x => $"[\"{x.id}\",\"{x.home_id}\",\"{x.dona_id}\",\"{x.lloc}\",\"{x.data}\",\"{x.observacions}\"]");
            }
            return GetAsHtml(title, header, rows);
        }

        private string GetPersonesResults(string title, bool emptyRows)
        {
            var header = $"[\"search_key\",\"id\",\"nom\",\"llinatge_1\",\"llinatge_2\",\"home\",\"naixement_lloc\",\"naixement_data\",\"mort_lloc\",\"mort_data\",\"info\",\"observacions\"]";
            var rows = default(IEnumerable<string>);
            if (!emptyRows)
            {
                var persones = _personaRepository.GetAll();
                rows = persones.Select(x => $"[\"{x.GetSearchKey()}\",\"{x.id}\",\"{x.nom}\",\"{x.llinatge_1}\",\"{x.llinatge_2}\",\"{x.home}\",\"{x.naixement_lloc}\",\"{x.naixement_data}\",\"{x.mort_lloc}\",\"{x.mort_data}\",\"{x.info}\",\"{x.observacions}\"]");
            }
            return GetAsHtml(title, header, rows);
        }

    }
}
