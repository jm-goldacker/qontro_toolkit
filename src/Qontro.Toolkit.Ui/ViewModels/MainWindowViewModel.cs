using System.Threading.Tasks;
using System.Timers;
using Qontro.Toolkit.Logic;

namespace Qontro.Toolkit.Ui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Url
    {
        get => _url;
        set
        {
            _user = value;
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
        get
        {
            return _progress;
        }
        set
        {
            _progress = value;
            OnPropertyChanged();
        }
    }
    
    public int ProgressMaximum
    {
        get
        {
            return _progressMaximum;
        }
        set
        {
            _progressMaximum = value;
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

    public bool NeedsLogin
    {
        get => _needsLogin;
        set
        {
            _needsLogin = value;
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
        
        IsExportPossible = true;
        NeedsLogin = false;
    }

    public async Task ExportCreditors()
    {
        await Task.Run(_accountProcessor.ExportCreditors);
    }
    
    public async Task ExportSuppliers()
    {
        await Task.Run(_accountProcessor.ExportSuppliers);
    }

    private string _url = "https://www14.qontro.com/";
    private string _user;
    private string _password;
    private int _progress;
    private int _progressMaximum;

    private AccountProcessor _accountProcessor;
    private bool _isExportPossible;
    private bool _needsLogin = true;

}