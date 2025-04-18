using Qontro.Toolkit.Logic.Events;

namespace Qontro.Toolkit.Logic.Export;

public interface IAccountExport
{
    public event EventHandler<RowsCountChangedEventArgs>? RowsCountChanged;
    public event EventHandler<CurrentProcessingItemChangedEventArgs>? CurrentProcessingItemChanged;
    public void Export(string filePath);
}