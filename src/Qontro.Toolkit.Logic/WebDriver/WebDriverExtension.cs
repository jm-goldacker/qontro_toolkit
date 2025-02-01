using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Qontro.Toolkit.Logic.WebDriver;

public static class WebDriverExtension
{
    public static Task Login(this IWebDriver webDriver, string url, string username, string password)
    {
        webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        webDriver.Navigate().GoToUrl(url);
        var submitButton = webDriver.FindElement(By.Name("Login"));
        var userField = webDriver.FindElement(By.Name("User__Name"));
        var passField = webDriver.FindElement(By.Name("User__Pass"));
        userField.SendKeys(username);
        passField.SendKeys(password);
        submitButton.Click();

        WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(5));
        var success = wait.Until(d => d.Url.Contains("eAccounts_Main.asp"));
        return Task.FromResult(success);
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
    
    public static void NavigateToSupplierEnquiry(this IWebDriver webDriver)
    {
        var creditorsMenu = webDriver.FindElement(By.Id("Stock"));
        creditorsMenu.Click();
        var enquiryButton = webDriver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[2];
        enquiryButton.Click();
    }
    
    public static void ClickMaintainSupplierButton(this IWebDriver webDriver)
    {
        var maintainButton = webDriver.FindElement(By.Name("Maintain_Supplier"));
        maintainButton.Click();
    }

    public static void ClearSearch(this IWebDriver webDriver)
    {
        var clearButton = webDriver.FindElement(By.Name("clear"));
        clearButton.Click();
    }
    
    public static void EnterCreditorCode(this IWebDriver webDriver, string code)
    {
        var inputField = webDriver.FindElement(By.Name("cAcct__Code"));
        inputField.SendKeys(code);
    }

    public static void EnterSupplierCode(this IWebDriver webDriver, string code)
    {
        var inputField = webDriver.FindElement(By.Name("sSupp__Code"));
        inputField.SendKeys(code);
    }
    
    public static void StartSearch(this IWebDriver webDriver)
    {
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

    public static void ClickSaveButton(this IWebDriver webDriver)
    {
        webDriver.FindElement(By.Name("save")).Click();
    }
}