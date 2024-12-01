namespace Qontro.Toolkit.DataAccess;

public class CsvWriter(
    IEnumerable<string> exportFieldNames,
    IEnumerable<string> headers,
    IEnumerable<IEnumerable<string>> exportRows)
{
    public void SaveAsCsv(Stream fileStream)
    {
        using var fw = new StreamWriter(fileStream);
        
        fw.WriteLine(string.Join(';', exportFieldNames));
        fw.WriteLine(string.Join(';', headers));
        
        foreach (var cred in exportRows)
        {
            fw.WriteLine(string.Join(";", cred));
        }

        fw.Flush();
        fw.Close();
    }
}