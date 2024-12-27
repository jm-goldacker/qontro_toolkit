namespace Qontro.Toolkit.Logic.Events;

public class CurrentProcessingItemChangedEventArgs(int currentItem) : EventArgs
{
    public int CurrentItem { get; private set; } = currentItem;
}