using System.Collections.ObjectModel;
using OpenQA.Selenium;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Export;

public class CreditorExport(ISeleniumWebDriver webDriver) : AccountExport(webDriver), ICreditorExport
{
    protected override void NavigateToAccount()
    {
        WebDriver.NavigateToCreditorEnquiry();
    }

    protected override void InitExportFields()
    {
        ExportRow = new List<string>();
        ExportFieldNames = ["", "", "", "", ""];
        Headers = new List<string>();
    }

    protected override void ExportMetadata(ReadOnlyCollection<IWebElement> columns)
    {
        Headers.Add("Last Payment");
        ExportRow.Add(columns[2].Text);

        Headers.Add("Balance Owing");
        ExportRow.Add(columns[8].Text);
    }

    protected override bool ExportLastUpdated()
    {
        Headers.Add("Date And Time");
        Headers.Add("User");
        
        var changesButton = WebDriver.FindElement(By.Name("Show_Changes"));
        var isChangesButtonActive = changesButton.Enabled;
        changesButton.Click();

        if (isChangesButtonActive)
        {
            var changesTable = WebDriver.FindElements(By.TagName("table"))[1];
            var changesRow = changesTable.FindElements(By.TagName("tr"));

            var lastUpdate = changesRow[^2];
            var lastUpdateColumns = lastUpdate.FindElements(By.TagName("td"));
                
            ExportRow.Add(lastUpdateColumns[1].Text);
            ExportRow.Add(lastUpdateColumns[3].Text);
        }
        else
        {
            ExportRow.Add(string.Empty);
            ExportRow.Add(string.Empty);
        }

        return isChangesButtonActive;
    }
    
    protected override void ClickMaintainButton()
    {
        WebDriver.ClickMaintainCreditorButton();
    }
}