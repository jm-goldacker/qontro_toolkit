using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace Qontro.Toolkit.Logic;

public class Export
{
    private readonly IWebDriver _driver;
    private readonly List<List<string>> _exportRows = new();
    private List<string> _headers = new();
    private List<string> _exportRow = new();
    private List<string> _exportFieldNames = new();
    private readonly string _url;
    private readonly string _user;
    private readonly string _password;

    public Export(string url, string user, string password)
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        _driver = new ChromeDriver(service);

        _url = url;
        _user = user;
        _password = password;
    }

    public event EventHandler<RowsCountChangedEventArgs>? RowsCountChanged;
    public event EventHandler<CurrentProcessingItemChangedEventArgs>? CurrentProcessingItemChanged;

    public void Login()
    {
        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        _driver.Navigate().GoToUrl(_url);
        var submitButton = _driver.FindElement(By.Name("Login"));
        var userField = _driver.FindElement(By.Name("User__Name"));
        var passField = _driver.FindElement(By.Name("User__Pass"));
        userField.SendKeys(_user);
        passField.SendKeys(_password);
        submitButton.Click();
    }

    // TODO Check Login
    public void ExportCreditors()
    {
        var creditorsMenu = _driver.FindElement(By.Id("Creditors"));
        creditorsMenu.Click();
        var enquiryButton = _driver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[1];
        enquiryButton.Click();
        var iSearchButton = _driver.FindElement(By.Name("Search2"));
        iSearchButton.Click();

        Thread.Sleep(2000);
        var rowsCount = _driver.FindElements(By.TagName("tr")).Skip(2).ToList().Count;
        RowsCountChanged?.Invoke(this, new RowsCountChangedEventArgs(rowsCount));
        
        for (var i = 0; i < rowsCount; i++)
        {
            CurrentProcessingItemChanged?.Invoke(this, new CurrentProcessingItemChangedEventArgs(i + 1));
            var row = _driver.FindElements(By.TagName("tr")).Skip(2 + i).First();
            if (row.GetAttribute("class").Contains("footer")) continue;

            _exportRow = new List<string>();
            _exportFieldNames = new List<string>() {"", "", "", "", ""};
            _headers = new List<string>();
            
            var columns = row.FindElements(By.TagName("td"));
            
            _headers.Add("Last Payment");
            _exportRow.Add(columns[2].Text);

            _headers.Add("Balance Owing");
            _exportRow.Add(columns[8].Text);

            var firstColumn = columns.First();
            var link = firstColumn.FindElement(By.TagName("a"));
            var creditorLinkHref = link.GetAttribute("href");
            link.Click();

            _headers.Add("Date And Time");
            _headers.Add("User");
            
            var changesButton = _driver.FindElement(By.Name("Show_Changes"));
            var isChangesButtonActive = changesButton.Enabled;
            changesButton.Click();

            if (isChangesButtonActive)
            {
                var changesTable = _driver.FindElements(By.TagName("table"))[1];
                var changesRow = changesTable.FindElements(By.TagName("tr"));

                var lastUpdate = changesRow[^2];
                var lastUpdateColumns = lastUpdate.FindElements(By.TagName("td"));
                
                _exportRow.Add(lastUpdateColumns[1].Text);
                _exportRow.Add(lastUpdateColumns[3].Text);
            }
            else
            {
                _exportRow.Add(string.Empty);
                _exportRow.Add(string.Empty);
            }
            
            _headers.Add("Link");
            _exportRow.Add($"=HYPERLINK(\"{creditorLinkHref}\")");

            var maintainButton = _driver.FindElement(By.Name("Maintain_Creditor"));
            maintainButton.Click();
            
            ExportForm();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            
            _exportRows.Add(_exportRow);

            if (isChangesButtonActive)
            {
                _driver.Navigate().Back();
            }

            _driver.Navigate().Back();
            _driver.Navigate().Back();
        }
        
        var csv = File.Open("./creditors.csv", FileMode.Create);
        using var fw = new StreamWriter(csv);
        
        fw.WriteLine(string.Join(';', _exportFieldNames));
        fw.WriteLine(string.Join(';', _headers));
        
        foreach (var cred in _exportRows)
        {
            fw.WriteLine(string.Join(";", cred));
        }

        fw.Flush();
        fw.Close();
        
        _driver.Navigate().Back();
        _driver.Navigate().Back();
        _driver.Navigate().Back();
    }

    // TODO Check Login
    public void ExportSuppliers()
    {
        var creditorsMenu = _driver.FindElement(By.Id("Stock"));
        creditorsMenu.Click();
        var enquiryButton = _driver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[2];
        enquiryButton.Click();
        var iSearchButton = _driver.FindElement(By.Name("Search2"));
        iSearchButton.Click();
        
        Thread.Sleep(2000);
        var rowsCount = _driver.FindElements(By.TagName("tr")).Skip(2).ToList().Count;
        RowsCountChanged?.Invoke(this, new RowsCountChangedEventArgs(rowsCount));
        
        for (var i = 0; i < rowsCount; i++)
        {
            CurrentProcessingItemChanged?.Invoke(this, new CurrentProcessingItemChangedEventArgs(i + 1));
            
            var row = _driver.FindElements(By.TagName("tr")).Skip(2 + i).First();
            if (row.GetAttribute("class").Contains("footer")) continue;
            
            _exportRow = new List<string>();
            _exportFieldNames = new List<string>() {"", ""};
            _headers = new List<string>();
            
            var columns = row.FindElements(By.TagName("td"));

            _headers.Add("Linked Stock");
            _exportRow.Add(columns.Last().Text);

            var firstColumn = columns.First();
            var link = firstColumn.FindElement(By.TagName("a"));
            
            _headers.Add("Link");
            _exportRow.Add($"=HYPERLINK(\"{link.GetAttribute("href")}\")");
            
            link.Click();

            var maintainButton = _driver.FindElement(By.Name("Maintain_Supplier"));
            maintainButton.Click();

            ExportForm();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            
            _exportRows.Add(_exportRow);
            _driver.Navigate().Back();
            _driver.Navigate().Back();
        }

        var csv = File.Open("./suppliers.csv", FileMode.Create);
        using var fw = new StreamWriter(csv);
        fw.WriteLine(string.Join(';', _exportFieldNames));
        fw.WriteLine(string.Join(';', _headers));

        foreach (var cred in _exportRows)
        {
            fw.WriteLine(string.Join(";", cred));
        }

        fw.Flush();
        fw.Close();
        
        _driver.Navigate().Back();
        _driver.Navigate().Back();
        _driver.Navigate().Back();
    }


    void ExportForm()
    {
        var reachedBankAccountTable = false;
        var table = _driver.FindElement(By.TagName("table"));
        var rows = table.FindElements(By.TagName("tr"));

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

        foreach (var row in rows)
        {
            ExportFormRow(row, ref reachedBankAccountTable);
        }
    }

    void ExportFormRow(IWebElement row, ref bool reachedBankAccountTable)
    {
        if (row.FindElements(By.TagName("table")).Count != 0)
        {
            reachedBankAccountTable = true;
            return;
        }
        var columns = row.FindElements(By.TagName("td"));

        foreach (var column in columns)
        {
            ExportColumn(column);
        }
        
        if (_exportRow.Count < _headers.Count && !reachedBankAccountTable)
        {
            _exportRow.Add(string.Empty);
        }

        if (_exportFieldNames.Count < _exportRow.Count)
        {
            _exportFieldNames.Add(string.Empty);
        }
    }

    void ExportColumn(IWebElement column)
    {
        if (column.GetAttribute("class") == "label")
        {
            _headers.Add(column.Text.Replace("\r\n", " "));
        }

        else
        {
            var elements = column.FindElements(By.XPath(".//*"));

            foreach (var element in elements)
            {
                if (element.GetAttribute("class") == "label")
                {
                    _headers.Add(element.Text.Replace("\r\n", " "));
                }
                else switch (element.TagName)
                {
                    case "input" when element.GetAttribute("type") == "text":
                    case "input" when element.GetAttribute("type") == "radio" && element.Selected:
                        _exportRow.Add(element.GetAttribute("value"));
                        _exportFieldNames.Add(element.GetAttribute("name"));
                        break;
                    case "select":
                    {
                        var selectedOption = element.FindElements(By.TagName("option")).FirstOrDefault(o => o.Selected);
                        _exportRow.Add(selectedOption?.Text ?? string.Empty);
                        _exportFieldNames.Add(element.GetAttribute("name"));
                        break;
                    }
                }

                if (_headers.Count < _exportRow.Count) _headers.Add(string.Empty);
            }
        }
    }
}