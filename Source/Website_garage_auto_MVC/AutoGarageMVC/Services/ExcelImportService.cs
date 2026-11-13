namespace AutoGarageMVC.Services;

using ClosedXML.Excel;
using System.IO;

public class ExcelImportService
{
    public List<Dictionary<string, string>> ReadFirstSheet(Stream stream)
    {
        var result = new List<Dictionary<string, string>>();
        using var wb = new XLWorkbook(stream);
        var ws = wb.Worksheets.FirstOrDefault();
        if (ws == null) return result;

        var headers = ws.Row(1).CellsUsed().Select(c => c.GetString().Trim()).ToList();
        foreach (var row in ws.RowsUsed().Skip(1))
        {
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < headers.Count; i++)
            {
                dict[headers[i]] = row.Cell(i + 1).GetString();
            }
            result.Add(dict);
        }
        return result;
    }
}
