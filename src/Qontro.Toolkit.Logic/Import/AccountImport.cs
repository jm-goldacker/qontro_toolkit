using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Qontro.Toolkit.DataAccess;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Import;

public abstract class AccountImport(ISeleniumWebDriver webDriver) : IAccountImport
{
    protected readonly ISeleniumWebDriver WebDriver = webDriver;

    public void Import(string filePath)
    {
        var csvReader = new CsvReader(filePath);
        var fieldNames = csvReader.FieldNames.ToList();
        var fieldOffset = GetCsvFieldOffset();
        
        NavigateToEnquiry();
        
        foreach (var fieldValues in csvReader.Data)
        {
            var creditorCode = fieldValues[fieldOffset];
            
            ClearAndSearchAccount(creditorCode);
            ClickMaintainButton();
            Import(fieldValues, fieldNames);
            WebDriver.ClickSaveButton();
            WebDriver.NavigateBackToOverview();
        }

        ClickMainMenuButton();
    }
    
    protected abstract int GetCsvFieldOffset();
    
    protected abstract void NavigateToEnquiry();
    
    private void ClearAndSearchAccount(string code)
    {
        WebDriver.ClearSearch();
        EnterCode(code);
        WebDriver.StartSearch();
    }
    
    protected abstract void EnterCode(string code);
    
    protected abstract void ClickMaintainButton();

    protected abstract void ClickMainMenuButton();

    private void Import(List<string> fieldValues, List<string> fieldNames)
    {
        var offset = GetCsvFieldOffset();
        
        for (var i = offset + 1; i < fieldValues.Count && i < fieldNames.Count(); i++)
        {
            IWebElement? element;
            try
            {
                element = WebDriver.FindElement(By.Name(fieldNames[i]));
            }
            catch (NoSuchElementException)
            {
                element = null;
            }
            
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
                var radioButton = WebDriver.FindElements(By.Name(fieldName))
                    .FirstOrDefault(b => b.GetAttribute("value") == value);
                radioButton?.Click();
                break;
            case "select":
                var selectElement = new SelectElement(element);
                selectElement.SelectByText(value);
                break;
            
        }
    }
}