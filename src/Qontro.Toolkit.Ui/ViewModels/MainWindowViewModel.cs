﻿using System.IO;
using System.Threading.Tasks;
using Qontro.Toolkit.Logic.Events;
using Qontro.Toolkit.Logic.Export;
using Qontro.Toolkit.Logic.Import;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Ui.ViewModels;

// ReSharper disable once PartialTypeWithSinglePart
public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ISeleniumWebDriver _seleniumWebDriver;
    private readonly ICreditorExport _creditorExport;
    private readonly ISupplierExport _supplierExport;
    private readonly ICreditorImport _creditorImport;
    private readonly ISupplierImport _supplierImport;

    public MainWindowViewModel(ISeleniumWebDriver seleniumWebDriver, ICreditorExport creditorExport,
        ISupplierExport supplierExport, ICreditorImport creditorImport, ISupplierImport supplierImport)
    {
        _seleniumWebDriver = seleniumWebDriver;
        _creditorExport = creditorExport;
        _supplierExport = supplierExport;
        _creditorImport = creditorImport;
        _supplierImport = supplierImport;
    }
    
    public string Url
    {
        get => _url;
        set
        {
            _url = value;
            OnPropertyChanged();
        }
    }

    public string User
    {
        get => _user;
        set
        {
            _user = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }
    
    public int Progress
    {
        get => _progress;
        set
        {
            _progress = value;
            OnPropertyChanged();
        }
    }
    
    public int ProgressMaximum
    {
        get => _progressMaximum;
        set
        {
            _progressMaximum = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsLoginNeeded
    {
        get => _isLoginNeeded;
        set
        {
            _isLoginNeeded = value;
            OnPropertyChanged();

            if (IsLoginNeeded && ExportFileStream != null)
            {
                IsExportPossible = true;
            }
        }
    }
    
    public Stream? ExportFileStream
    {
        get => _exportFileStream;
        set
        {
            _exportFileStream?.Dispose();
            _exportFileStream = value;
            OnPropertyChanged();

            if (ExportFileStream != null && !IsLoginNeeded)
            {
                IsExportPossible = true;
            }
        }
    }
    
    public Stream? ImportFileStream
    {
        get => _importFileStream;
        set
        {
            _importFileStream?.Dispose();
            _importFileStream = value;
            OnPropertyChanged();

            if (ImportFileStream != null && !IsLoginNeeded)
            {
                IsImportPossible = true;
            }
        }
    }
    
    public string? ExportFilePath
    {
        get => _exportFilePath;
        set
        {
            _exportFilePath = value ?? "no file selected";
            OnPropertyChanged();
        }
    }
    
    public string? ImportFilePath
    {
        get => _importFilePath;
        set
        {
            _importFilePath = value ?? "no file selected";
            OnPropertyChanged();
            
            if (ImportFilePath != null && !IsLoginNeeded)
            {
                IsImportPossible = true;
            }
        }
    }

    public bool IsExportPossible
    {
        get => _isExportPossible;
        set
        {
            _isExportPossible = value;
            OnPropertyChanged();
        }
    }
    
    public bool IsImportPossible
    {
        get => _isImportPossible;
        set
        {
            _isImportPossible = value;
            OnPropertyChanged();
        }
    }
    
    public async Task Login()
    {
        if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password)) return;
        var isSuccessful = false;
        await Task.Run(() => _seleniumWebDriver.Login(Url, User, Password));
        IsLoginNeeded = !isSuccessful;
    }

    public async Task ExportCreditors()
    {
        if (ExportFileStream != null)
        {
            await RunExport(_creditorExport);
        }
    }

    public async Task ExportSuppliers()
    {
        if (ExportFileStream != null)
        {
            await RunExport(_supplierExport);
        }
    }
    
    private async Task RunExport(IAccountExport creditorExport)
    {
        creditorExport.RowsCountChanged += OnCreditorExportOnRowsCountChanged;
        creditorExport.CurrentProcessingItemChanged += OnCreditorExportOnCurrentProcessingItemChanged;
        await Task.Run(() => creditorExport.Export(ExportFileStream!));
        creditorExport.RowsCountChanged -= OnCreditorExportOnRowsCountChanged;
        creditorExport.CurrentProcessingItemChanged -= OnCreditorExportOnCurrentProcessingItemChanged;
    }

    public async Task ImportCreditors()
    {
        if (ImportFilePath != null)
        {
            await Task.Run(() => _creditorImport.Import(ImportFilePath));
        }
    }

    public async Task ImportSuppliers()
    {
        if (ImportFilePath != null)
        {
            await Task.Run(() => _supplierImport.Import(ImportFilePath));
        }
    }

    private string _url = "https://www14.qontro.com/";
    private string _user = string.Empty;
    private string _password = string.Empty;
    private int _progress;
    private int _progressMaximum;

    private bool _isLoginNeeded = true;
    private Stream? _exportFileStream;
    private Stream? _importFileStream;
    private string? _exportFilePath = "no file selected";
    private string? _importFilePath = "no file selected";
    private bool _isExportPossible;
    private bool _isImportPossible;
    
    private void OnCreditorExportOnCurrentProcessingItemChanged(object? _, CurrentProcessingItemChangedEventArgs e)
    {
        Progress = e.CurrentItem;
    }

    private void OnCreditorExportOnRowsCountChanged(object? _, RowsCountChangedEventArgs e)
    {
        ProgressMaximum = e.RowsCount;
    }
    
}