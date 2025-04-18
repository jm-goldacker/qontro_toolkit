namespace Qontro.Toolkit.DataAccess;

public class CsvWriter(
    IEnumerable<string> exportFieldNames,
    IEnumerable<string> headers,
    IEnumerable<IEnumerable<string>> exportRows)
{
    public void SaveAsCsv(string filePath)
    {
        using var fw = new StreamWriter(filePath);
        
        fw.WriteLine(string.Join(';', exportFieldNames));
        fw.WriteLine(string.Join(';', headers));
        
        foreach (var exportRow in exportRows)
        {
            var values = exportRow.ToList();
            values = values.Select(c => c.Replace(";", "#0#").Replace("'", "#1#")).ToList();
            fw.WriteLine(string.Join(";", values));
        }

        fw.Flush();
        fw.Close();
    }
}