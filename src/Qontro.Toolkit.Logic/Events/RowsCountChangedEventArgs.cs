namespace Qontro.Toolkit.Logic.Events;

public class RowsCountChangedEventArgs(int rowsCount) : EventArgs
{
    public int RowsCount { get; private set; } = rowsCount;
}