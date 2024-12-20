namespace Qontro.Toolkit.DataAccess;

public class CsvReader
{
    public CsvReader(string filePath)
    {
        ExportRows = new List<List<string>>();
        using var streamReader = new StreamReader(filePath);
        
        ExportFieldNames = streamReader.ReadLine()?.Split(",").ToList() ?? 
                           throw new InvalidDataException("no export field names found");
        
        streamReader.ReadLine();

        while (streamReader.ReadLine() is { } line)
        {
            var row = line.Split(",");
            ExportRows.Add(row.ToList());
        }
        
        streamReader.Close();
    }
    
    public IEnumerable<string> ExportFieldNames { get; private init; }
    public List<List<string>> ExportRows { get; }
}