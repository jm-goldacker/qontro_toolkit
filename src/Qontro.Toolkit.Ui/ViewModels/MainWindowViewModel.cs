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

            if (IsLoginNeeded && FileStream != null)
            {
                IsExportPossible = true;
            }
        }
    }
    
    public Stream? FileStream
    {
        get => _fileStream;
        set
        {
            _fileStream?.Dispose();
            _fileStream = value;
            OnPropertyChanged();

            if (FileStream != null && !IsLoginNeeded)
            {
                IsExportPossible = true;
            }
        }
    }
    
    public string? FilePath
    {
        get => _filePath;
        set
        {
            _filePath = value ?? "no file selected";
            OnPropertyChanged();
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
        if (FileStream != null)
        {
            await Task.Run(() => _accountProcessor?.ExportCreditors(FileStream));
        }
    }

    public async Task ExportSuppliers()
    {
        if (FileStream != null)
        {
            await Task.Run(() => _accountProcessor?.ExportSuppliers(FileStream));
        }
    }

    public async Task ImportCreditors()
    {
        await Task.Run(() => _accountProcessor?.ImportCreditor());
    }
    
    public async Task ImportSuppliers() {}

    private string _url = "https://www14.qontro.com/";
    private string _user = string.Empty;
    private string _password = string.Empty;
    private int _progress;
    private int _progressMaximum;

    private AccountProcessor? _accountProcessor;
    private bool _isLoginNeeded = true;
    private Stream? _fileStream;
    private string? _filePath = "no file selected";
    private bool _isExportPossible;
}