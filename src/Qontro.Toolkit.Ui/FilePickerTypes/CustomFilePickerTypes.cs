using Avalonia.Platform.Storage;

namespace Qontro.Toolkit.Ui.FilePickerTypes;

public static class CustomFilePickerTypes
{
    public static FilePickerFileType Csv { get; } = new("csv files")
    {
        Patterns = ["*.csv"],
        AppleUniformTypeIdentifiers = [" public.comma-separated-values-text"],
        MimeTypes = ["text/csv"]
    };
}