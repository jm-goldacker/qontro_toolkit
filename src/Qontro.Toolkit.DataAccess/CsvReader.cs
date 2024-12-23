namespace Qontro.Toolkit.DataAccess;

public class CsvReader
{
    public CsvReader(string filePath)
    {
        Data = new List<List<string>>();
        using var streamReader = new StreamReader(filePath);
        
        FieldNames = streamReader.ReadLine()?.Split(",").ToList() ?? 
                           throw new InvalidDataException("no export field names found");
        
        streamReader.ReadLine();

        while (streamReader.ReadLine() is { } line)
        {
            var row = line.Split(",");
            Data.Add(row.ToList());
        }
        
        streamReader.Close();
    }
    
    public IEnumerable<string> FieldNames { get; private init; }
    public List<List<string>> Data { get; }
}