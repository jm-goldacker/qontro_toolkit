using System.IO;
using System.Threading.Tasks;
using Qontro.Toolkit.Logic;

namespace Qontro.Toolkit.Ui.ViewModels;

// ReSharper disable once PartialTypeWithSinglePart
public partial class MainWindowViewModel : ViewModelBase
{
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
        _accountProcessor = new AccountProcessor(Url, User, Password);
        _accountProcessor.RowsCountChanged += (_, e) => ProgressMaximum = e.RowsCount;
        _accountProcessor.CurrentProcessingItemChanged += (_, e) => Progress = e.CurrentItem;
        await Task.Run(_accountProcessor.Login);
        
        IsLoginNeeded = false;
    }

    public async Task ExportCreditors()
    {
        if (ExportFileStream != null)
        {
            await Task.Run(() => _accountProcessor?.ExportCreditors(ExportFileStream));
        }
    }

    public async Task ExportSuppliers()
    {
        if (ExportFileStream != null)
        {
            await Task.Run(() => _accountProcessor?.ExportSuppliers(ExportFileStream));
        }
    }

    public async Task ImportCreditors()
    {
        if (ImportFilePath != null)
        {
            await Task.Run(() => _accountProcessor?.ImportCreditor(ImportFilePath));
        }
    }
    
    public async Task ImportSuppliers() {}

    private string _url = "https://www14.qontro.com/";
    private string _user = string.Empty;
    private string _password = string.Empty;
    private int _progress;
    private int _progressMaximum;

    private AccountProcessor? _accountProcessor;
    private bool _isLoginNeeded = true;
    private Stream? _exportFileStream;
    private Stream? _importFileStream;
    private string? _exportFilePath = "no file selected";
    private string? _importFilePath = "no file selected";
    private bool _isExportPossible;
    private bool _isImportPossible;
}