namespace Qontro.Toolkit.DataAccess;

public class CsvReader
{
    public CsvReader(string filePath)
    {
        Data = new List<List<string>>();
        using var file = File.OpenRead(filePath);
        using var streamReader = new StreamReader(file);
        
        FieldNames = streamReader.ReadLine()?.Split(",") ?? 
                           throw new InvalidDataException("no export field names found");
        FieldNames = FieldNames.Select(fn => fn.Replace("#0#", ";").Replace("#1#", ",")).ToList();
        
        streamReader.ReadLine();

        while (streamReader.ReadLine() is { } line)
        {
            var row = line.Split(",");
            Data.Add(row.Select(f => f.Replace("#0#", ";").Replace("#1#", ",")).ToList());
        }
        
        streamReader.Close();
    }
    
    public IEnumerable<string> FieldNames { get; private init; }
    public List<List<string>> Data { get; }
}