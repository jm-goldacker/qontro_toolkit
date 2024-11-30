namespace Qontro.Toolkit.Logic;

public class CurrentProcessingItemChangedEventArgs(int currentItem) : EventArgs
{
    public int CurrentItem { get; private set; } = currentItem;
}