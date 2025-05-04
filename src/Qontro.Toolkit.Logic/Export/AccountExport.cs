using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Qontro.Toolkit.DataAccess;
using Qontro.Toolkit.Logic.Events;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Export;

public abstract class AccountExport(ISeleniumWebDriver webDriver) : IAccountExport
{
    private readonly List<List<string>> _exportRows = new();
    protected List<string> Headers = new();
    protected List<string> ExportRow = new();
    protected List<string> ExportFieldNames = new();
    protected ISeleniumWebDriver WebDriver = webDriver;

    public event EventHandler<RowsCountChangedEventArgs>? RowsCountChanged;
    public event EventHandler<CurrentProcessingItemChangedEventArgs>? CurrentProcessingItemChanged;

    public void Export(string filePath)
    {
        NavigateToAccount();
        WebDriver.ClearSearch();
        WebDriver.StartSearch();
        
        var rowsCount = GetRowsCount();
        
        for (var i = 0; i < rowsCount; i++)
        {
            CurrentProcessingItemChanged?.Invoke(this, new CurrentProcessingItemChangedEventArgs(i + 1));
            
            var row = WebDriver.FindElements(By.TagName("tr")).Skip(2 + i).First();
            if (row.GetAttribute("class").Contains("footer")) continue;

            InitExportFields();
            
            var columns = row.FindElements(By.TagName("td"));

            ExportMetadata(columns);
            var creditorLinkHref = GetAndNavigateToLink(columns);
            var isChangesButtonActive = ExportLastUpdated();
            ExportLink(creditorLinkHref);

            ClickMaintainButton();

            ExportForm();
            WebDriver.SetTimeout(TimeSpan.FromSeconds(30));
            
            _exportRows.Add(ExportRow);
            
            if (isChangesButtonActive)
            {
                WebDriver.NavigateBack();
            }
            
            WebDriver.NavigateBackToOverview();
        }

        var csvWriter = new CsvWriter(ExportFieldNames, Headers, _exportRows);
        csvWriter.SaveAsCsv(filePath);
        
        WebDriver.NavigateBackToMenu();
    }

    protected abstract void NavigateToAccount();
    protected abstract void InitExportFields();
    protected abstract void ExportMetadata(ReadOnlyCollection<IWebElement> columns);
    protected abstract bool ExportLastUpdated();
    protected abstract void ClickMaintainButton();

    private int GetRowsCount()
    {
        Thread.Sleep(2000);
        var rowsCount = WebDriver.FindElements(By.TagName("tr")).Skip(2).ToList().Count;
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
        var table = WebDriver.FindElement(By.TagName("table"));
        var rows = table.FindElements(By.TagName("tr"));

        WebDriver.SetTimeout(TimeSpan.FromSeconds(0));

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