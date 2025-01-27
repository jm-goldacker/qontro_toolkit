using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Qontro.Toolkit.DataAccess;
using Qontro.Toolkit.Logic.Events;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Export;

public abstract class AccountExport
{
    private readonly List<List<string>> _exportRows = new();
    protected List<string> Headers = new();
    protected List<string> ExportRow = new();
    protected List<string> ExportFieldNames = new();

    public event EventHandler<RowsCountChangedEventArgs>? RowsCountChanged;
    public event EventHandler<CurrentProcessingItemChangedEventArgs>? CurrentProcessingItemChanged;

    public void Export(Stream fileStream)
    {
        NavigateToAccount();
        SeleniumWebDriver.Instance.ClearSearch();
        SeleniumWebDriver.Instance.StartSearch();
        
        var rowsCount = GetRowsCount();
        
        for (var i = 0; i < rowsCount; i++)
        {
            CurrentProcessingItemChanged?.Invoke(this, new CurrentProcessingItemChangedEventArgs(i + 1));
            
            var row = SeleniumWebDriver.Instance.FindElements(By.TagName("tr")).Skip(2 + i).First();
            if (row.GetAttribute("class").Contains("footer")) continue;

            InitExportFields();
            
            var columns = row.FindElements(By.TagName("td"));

            ExportMetadata(columns);
            var creditorLinkHref = GetAndNavigateToLink(columns);
            var isChangesButtonActive = ExportLastUpdated();
            ExportLink(creditorLinkHref);

            SeleniumWebDriver.Instance.ClickMaintainSupplierButton();

            ExportForm();
            SeleniumWebDriver.Instance.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
            
            _exportRows.Add(ExportRow);
            
            if (isChangesButtonActive)
            {
                SeleniumWebDriver.Instance.Navigate().Back();
            }
            
            SeleniumWebDriver.Instance.NavigateBackToOverview();
        }

        var csvWriter = new CsvWriter(ExportFieldNames, Headers, _exportRows);
        csvWriter.SaveAsCsv(fileStream);
        
        SeleniumWebDriver.Instance.NavigateBackToMenu();
    }

    protected abstract void NavigateToAccount();
    protected abstract void InitExportFields();
    protected abstract void ExportMetadata(ReadOnlyCollection<IWebElement> columns);
    protected abstract bool ExportLastUpdated();

    private int GetRowsCount()
    {
        Thread.Sleep(2000);
        var rowsCount = SeleniumWebDriver.Instance.FindElements(By.TagName("tr")).Skip(2).ToList().Count;
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
        Headers.Add("Link");
        ExportRow.Add($"=HYPERLINK(\"{creditorLinkHref}\")");
    }

    private void ExportForm()
    {
        var reachedBankAccountTable = false;
        var table = SeleniumWebDriver.Instance.FindElement(By.TagName("table"));
        var rows = table.FindElements(By.TagName("tr"));

        SeleniumWebDriver.Instance.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

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
        
        if (ExportRow.Count < Headers.Count && !reachedBankAccountTable)
        {
            ExportRow.Add(string.Empty);
        }

        if (ExportFieldNames.Count < ExportRow.Count)
        {
            ExportFieldNames.Add(string.Empty);
        }
    }

    private void ExportColumn(IWebElement column)
    {
        if (column.GetAttribute("class") == "label")
        {
            Headers.Add(column.Text.Replace("\r\n", " "));
            return;
        }

        var elements = column.FindElements(By.XPath(".//select | .//input | .//span"));

        foreach (var element in elements)
        {
            if (element.GetAttribute("class") == "label")
            {
                Headers.Add(element.Text.Replace("\r\n", " "));
                continue;
            }

            ExportInputField(element);

            if (Headers.Count < ExportRow.Count) 
                Headers.Add(string.Empty);
        }
    }

    private void ExportInputField(IWebElement element)
    {
        switch (element.TagName)
        {
            case "input" when element.GetAttribute("type") == "text":
            case "input" when element.GetAttribute("type") == "radio" && element.Selected:
                ExportRow.Add(element.GetAttribute("value"));
                ExportFieldNames.Add(element.GetAttribute("name"));
                break;
            case "select":
            {
                var select = new SelectElement(element);
                var selectedOption = select.SelectedOption;
                ExportRow.Add(selectedOption?.Text ?? string.Empty);
                ExportFieldNames.Add(element.GetAttribute("name"));
                break;
            }
        }
    }
}