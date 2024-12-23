using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Qontro.Toolkit.DataAccess;

namespace Qontro.Toolkit.Logic;

public class AccountProcessor
{
    private readonly IWebDriver _driver;
    private readonly List<List<string>> _exportRows = new();
    private List<string> _headers = new();
    private List<string> _exportRow = new();
    private List<string> _exportFieldNames = new();
    private readonly string _url;
    private readonly string _user;
    private readonly string _password;

    public AccountProcessor(string url, string user, string password)
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        var options = new ChromeOptions();
        options.AddArgument("--no-sandbox");
        _driver = new ChromeDriver(service, options);

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

    public void ImportCreditor(string filePath)
    {
        var csvReader = new CsvReader(filePath);
        var fieldNames = csvReader.FieldNames.ToList();
        
        NavigateToCreditorEnquiry();
        
        foreach (var fieldValues in csvReader.Data)
        {
            var creditorCode = fieldValues[5];
            ClearAndSearchAccount(creditorCode);
            ClickMaintainCreditorButton();
            Import(fieldValues, fieldNames);
            _driver.Navigate().Back();
        }
        
    }

    private void Import(List<string> fieldValues, List<string> fieldNames)
    {
        for (int i = 6; i < fieldValues.Count && i < fieldNames.Count(); i++)
        {
            var element = _driver.FindElement(By.Name(fieldNames[i]));
            if (element is null) continue;
                
            if (fieldValues[i] == "~")
            {
                RemoveValue(element);
            }
            else if (!string.IsNullOrEmpty(fieldValues[i]))
            {
                SetValue(element, fieldNames[i], fieldValues[i]);
            }
        }
    }

    private void RemoveValue(IWebElement element)
    {
        switch (element.TagName)
        {
            case "input" when element.GetAttribute("type") == "text":
                element.Clear();
                break;
        }
    }

    private void SetValue(IWebElement element, string fieldName, string value)
    {
        switch (element.TagName)
        {
            case "input" when element.GetAttribute("type") == "text":
                element.Clear();
                element.SendKeys(value);
                break;
            case "input" when element.GetAttribute("type") == "radio":
                var radioButton = _driver.FindElements(By.Name(fieldName))
                    .FirstOrDefault(b => b.GetAttribute("value") == value);
                radioButton?.Click();
                break;
            case "select":
                var selectElement = new SelectElement(element);
                selectElement.SelectByText(value);
                break;
            
        }
    }

    // TODO Check Login
    public void ExportCreditors(Stream fileStream)
    {
        NavigateToCreditorEnquiry();

        ClearAndSearchAccount();

        var rowsCount = GetRowsCount();

        for (var i = 0; i < rowsCount; i++)
        {
            CurrentProcessingItemChanged?.Invoke(this, new CurrentProcessingItemChangedEventArgs(i + 1));
            
            var row = _driver.FindElements(By.TagName("tr")).Skip(2 + i).First();
            if (row.GetAttribute("class").Contains("footer")) continue;

            _exportRow = new List<string>();
            _exportFieldNames = ["", "", "", "", ""];
            _headers = new List<string>();
            
            var columns = row.FindElements(By.TagName("td"));
            
            ExportPayment(columns);
            var creditorLinkHref = GetAndNavigateToLink(columns);
            var isChangesButtonActive = ExportLastUpdated();
            ExportLink(creditorLinkHref);

            ClickMaintainCreditorButton();

            ExportForm();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            
            _exportRows.Add(_exportRow);

            if (isChangesButtonActive)
            {
                _driver.Navigate().Back();
            }

            NavigateBackToOverview();
        }
        
        var csvWriter = new CsvWriter(_exportFieldNames, _headers, _exportRows);
        csvWriter.SaveAsCsv(fileStream);

        NavigateBackToMenu();
    }

    private void ClickMaintainCreditorButton()
    {
        var maintainButton = _driver.FindElement(By.Name("Maintain_Creditor"));
        maintainButton.Click();
    }

    private void NavigateToCreditorEnquiry()
    {
        var creditorsMenu = _driver.FindElement(By.Id("Creditors"));
        creditorsMenu.Click();
        var enquiryButton = _driver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[1]; 
        enquiryButton.Click();
    }

    private void ExportPayment(ReadOnlyCollection<IWebElement> columns)
    {
        _headers.Add("Last Payment");
        _exportRow.Add(columns[2].Text);

        _headers.Add("Balance Owing");
        _exportRow.Add(columns[8].Text);
    }

    private bool ExportLastUpdated()
    {
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

        return isChangesButtonActive;
    }
    
    // TODO Check Login
    public void ExportSuppliers(Stream fileStream)
    {
        NavigateToSuppliers();

        var rowsCount = GetRowsCount();
        
        for (var i = 0; i < rowsCount; i++)
        {
            CurrentProcessingItemChanged?.Invoke(this, new CurrentProcessingItemChangedEventArgs(i + 1));
            
            var row = _driver.FindElements(By.TagName("tr")).Skip(2 + i).First();
            if (row.GetAttribute("class").Contains("footer")) continue;
            
            _exportRow = new List<string>();
            _exportFieldNames = ["", ""];
            _headers = new List<string>();
            
            var columns = row.FindElements(By.TagName("td"));

            ExportLinkedStock(columns);
            var creditorLinkHref = GetAndNavigateToLink(columns);
            ExportLink(creditorLinkHref);

            var maintainButton = _driver.FindElement(By.Name("Maintain_Supplier"));
            maintainButton.Click();

            ExportForm();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            
            _exportRows.Add(_exportRow);
            NavigateBackToOverview();
        }

        var csvWriter = new CsvWriter(_exportFieldNames, _headers, _exportRows);
        csvWriter.SaveAsCsv(fileStream);
        
        NavigateBackToMenu();
    }

    private void NavigateToSuppliers(string? supplierCode = null)
    {
        var creditorsMenu = _driver.FindElement(By.Id("Stock"));
        creditorsMenu.Click();
        var enquiryButton = _driver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[2];
        enquiryButton.Click();
        
        ClearAndSearchAccount(supplierCode);
    }
    
    private void ExportLinkedStock(ReadOnlyCollection<IWebElement> columns)
    {
        _headers.Add("Linked Stock");
        _exportRow.Add(columns.Last().Text);
    }
    
    private void ClearAndSearchAccount(string? code = null)
    {
        var clearButton = _driver.FindElement(By.Name("clear"));
        clearButton.Click();

        if (!string.IsNullOrEmpty(code))
        {
            var inputField = _driver.FindElement(By.Name("cAcct__Code"));
            inputField.SendKeys(code);
        }
        
        var iSearchButton = _driver.FindElement(By.Name("Search2"));
        iSearchButton.Click();
    }
    
    private int GetRowsCount()
    {
        Thread.Sleep(2000);
        var rowsCount = _driver.FindElements(By.TagName("tr")).Skip(2).ToList().Count;
        RowsCountChanged?.Invoke(this, new RowsCountChangedEventArgs(rowsCount));
        return rowsCount;
    }
    
    private static string GetAndNavigateToLink(ReadOnlyCollection<IWebElement> columns)
    {
        var firstColumn = columns.First();
        var link = firstColumn.FindElement(By.TagName("a"));
        var creditorLinkHref = link.GetAttribute("href");
        link.Click();
        return creditorLinkHref;
    }
    
    private void ExportLink(string creditorLinkHref)
    {
        _headers.Add("Link");
        _exportRow.Add($"=HYPERLINK(\"{creditorLinkHref}\")");
    }

    private void ExportForm()
    {
        var reachedBankAccountTable = false;
        var table = _driver.FindElement(By.TagName("table"));
        var rows = table.FindElements(By.TagName("tr"));

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

        foreach (var row in rows)
        {
            if (row.FindElements(By.TagName("table")).Count != 0)
            {
                reachedBankAccountTable = true;
                continue;
            }
            
            ExportFormRow(row, reachedBankAccountTable);
        }
    }

    private void ExportFormRow(IWebElement row, bool reachedBankAccountTable)
    {
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

    private void ExportColumn(IWebElement column)
    {
        if (column.GetAttribute("class") == "label")
        {
            _headers.Add(column.Text.Replace("\r\n", " "));
            return;
        }

        var elements = column.FindElements(By.XPath(".//select | .//input | .//span"));

        foreach (var element in elements)
        {
            if (element.GetAttribute("class") == "label")
            {
                _headers.Add(element.Text.Replace("\r\n", " "));
                continue;
            }

            ExportInputField(element);

            if (_headers.Count < _exportRow.Count) 
                _headers.Add(string.Empty);
        }
    }

    private void ExportInputField(IWebElement element)
    {
        switch (element.TagName)
        {
            case "input" when element.GetAttribute("type") == "text":
            case "input" when element.GetAttribute("type") == "radio" && element.Selected:
                _exportRow.Add(element.GetAttribute("value"));
                _exportFieldNames.Add(element.GetAttribute("name"));
                break;
            case "select":
            {
                var select = new SelectElement(element);
                var selectedOption = select.SelectedOption;
                _exportRow.Add(selectedOption?.Text ?? string.Empty);
                _exportFieldNames.Add(element.GetAttribute("name"));
                break;
            }
        }
    }
    
    private void NavigateBackToOverview()
    {
        _driver.Navigate().Back();
        _driver.Navigate().Back();
    }
    
    private void NavigateBackToMenu()
    {
        _driver.Navigate().Back();
        _driver.Navigate().Back();
        _driver.Navigate().Back();
    }
}