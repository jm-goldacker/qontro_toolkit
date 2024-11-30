namespace Qontro.Toolkit.Logic;

public class RowsCountChangedEventArgs(int rowsCount) : EventArgs
{
    public int RowsCount { get; private set; } = rowsCount;
}