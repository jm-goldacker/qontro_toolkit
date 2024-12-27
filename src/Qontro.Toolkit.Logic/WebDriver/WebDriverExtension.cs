using OpenQA.Selenium;

namespace Qontro.Toolkit.Logic.WebDriver;

public static class WebDriverExtension
{
    public static void Login(this IWebDriver webDriver, string url, string username, string password)
    {
        webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        webDriver.Navigate().GoToUrl(url);
        var submitButton = webDriver.FindElement(By.Name("Login"));
        var userField = webDriver.FindElement(By.Name("User__Name"));
        var passField = webDriver.FindElement(By.Name("User__Pass"));
        userField.SendKeys(username);
        passField.SendKeys(password);
        submitButton.Click();
    }
    
    public static void NavigateToCreditorEnquiry(this IWebDriver webDriver)
    {
        var creditorsMenu = webDriver.FindElement(By.Id("Creditors"));
        creditorsMenu.Click();
        var enquiryButton = webDriver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[1]; 
        enquiryButton.Click();
    }
    
    public static void ClickMaintainCreditorButton(this IWebDriver webDriver)
    {
        var maintainButton = webDriver.FindElement(By.Name("Maintain_Creditor"));
        maintainButton.Click();
    }
    
    public static void NavigateToSuppliers(this IWebDriver webDriver, string? supplierCode = null)
    {
        var creditorsMenu = webDriver.FindElement(By.Id("Stock"));
        creditorsMenu.Click();
        var enquiryButton = webDriver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[2];
        enquiryButton.Click();
        
        webDriver.ClearAndSearchAccount(supplierCode);
    }
    
    public static void ClickMaintainSupplierButton(this IWebDriver webDriver)
    {
        var maintainButton = webDriver.FindElement(By.Name("Maintain_Supplier"));
        maintainButton.Click();
    }
    
    public static void ClearAndSearchAccount(this IWebDriver webDriver, string? code = null)
    {
        var clearButton = webDriver.FindElement(By.Name("clear"));
        clearButton.Click();

        if (!string.IsNullOrEmpty(code))
        {
            var inputField = webDriver.FindElement(By.Name("cAcct__Code"));
            inputField.SendKeys(code);
        }
        
        var iSearchButton = webDriver.FindElement(By.Name("Search2"));
        iSearchButton.Click();
    }
    
    public static void NavigateBackToOverview(this IWebDriver webDriver)
    {
        webDriver.Navigate().Back();
        webDriver.Navigate().Back();
    }
    
    public static void NavigateBackToMenu(this IWebDriver webDriver)
    {
        webDriver.Navigate().Back();
        webDriver.Navigate().Back();
        webDriver.Navigate().Back();
    }
}