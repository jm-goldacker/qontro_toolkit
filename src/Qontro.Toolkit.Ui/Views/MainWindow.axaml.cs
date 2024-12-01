using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Qontro.Toolkit.Ui.ViewModels;

namespace Qontro.Toolkit.Ui.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
                viewModel.FileStream = null;
                viewModel.FilePath = null;
                return;
            }

            var stream = await file.OpenWriteAsync();
            viewModel.FileStream = stream;
            viewModel.FilePath = file.Path.AbsolutePath;
        }
        catch (Exception exception)
        {
            throw; // TODO handle exception
        }
    }
}