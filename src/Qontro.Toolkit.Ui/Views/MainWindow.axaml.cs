using System;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Qontro.Toolkit.Ui.FilePickerTypes;
using Qontro.Toolkit.Ui.ViewModels;

namespace Qontro.Toolkit.Ui.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        VersionLabel.Content = "Version v" + 
                               Assembly.GetEntryAssembly()?
                                   .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    }

    private async void filePickerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not MainWindowViewModel viewModel) return;
            
            var topLevel = GetTopLevel(this);

            if (topLevel == null)
            {
                return;
            }
        
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save export",
                DefaultExtension = "csv"
            });

            if (file == null)
            {
                viewModel.ExportFileStream = null;
                viewModel.ExportFilePath = null;
                return;
            }

            var stream = await file.OpenWriteAsync();
            viewModel.ExportFileStream = stream;
            viewModel.ExportFilePath = file.Path.AbsolutePath;
        }
        catch (Exception exception)
        {
            throw; // TODO handle exception
        }
    }
    
    private async void importFilePickerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (DataContext is not MainWindowViewModel viewModel) return;
            
            var topLevel = GetTopLevel(this);

            if (topLevel == null)
            {
                return;
            }
        
            var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open import file",
                FileTypeFilter =
                [
                    CustomFilePickerTypes.Csv
                ]
            });

            var file = storageFiles.First();
            viewModel.ImportFilePath = file.Path.AbsolutePath;
        }
        catch (Exception exception)
        {
            throw; // TODO handle exception
        }
    }
}