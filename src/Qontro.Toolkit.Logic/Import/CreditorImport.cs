using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Qontro.Toolkit.DataAccess;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Import;

public class CreditorImport
{
    public void ImportCreditor(string filePath)
    {
        var csvReader = new CsvReader(filePath);
        var fieldNames = csvReader.FieldNames.ToList();
        
        SeleniumWebDriver.Instance.NavigateToCreditorEnquiry();
        
        foreach (var fieldValues in csvReader.Data)
        {
            var creditorCode = fieldValues[5];
            SeleniumWebDriver.Instance.ClearAndSearchAccount(creditorCode);
            SeleniumWebDriver.Instance.ClickMaintainCreditorButton();
            Import(fieldValues, fieldNames);
            SeleniumWebDriver.Instance.ClickSaveButton();
            SeleniumWebDriver.Instance.Navigate().Back();
            SeleniumWebDriver.Instance.Navigate().Back();
        }
        
        SeleniumWebDriver.Instance.FindElement(By.Id("menu-creditor-home")).Click();
    }

    private void Import(List<string> fieldValues, List<string> fieldNames)
    {
        for (int i = 6; i < fieldValues.Count && i < fieldNames.Count(); i++)
        {
            IWebElement? element;
            try
            {
                element = SeleniumWebDriver.Instance.FindElement(By.Name(fieldNames[i]));
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
                var radioButton = SeleniumWebDriver.Instance.FindElements(By.Name(fieldName))
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