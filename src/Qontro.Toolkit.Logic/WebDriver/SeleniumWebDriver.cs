using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Qontro.Toolkit.Logic.WebDriver;

public class SeleniumWebDriver : ISeleniumWebDriver
{
    private readonly IWebDriver _webDriver;
    
    public SeleniumWebDriver()
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        var options = new ChromeOptions();
        options.AddArgument("--no-sandbox");
        _webDriver = new ChromeDriver(service, options);
    }
    
    public bool Login(string url, string username, string password)
    {
        _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        _webDriver.Navigate().GoToUrl(url);
        var submitButton = _webDriver.FindElement(By.Name("Login"));
        var userField = _webDriver.FindElement(By.Name("User__Name"));
        var passField = _webDriver.FindElement(By.Name("User__Pass"));
        userField.SendKeys(username);
        passField.SendKeys(password);
        submitButton.Click();

        WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(5));
        try
        {
            var success = wait.Until(d => d.Url.Contains("eAccounts_Main.asp"));
            return success;
        }
        catch (WebDriverTimeoutException wdtExc)
        {
            return false;
        }
    }
    
    public void NavigateToCreditorEnquiry()
    {
        var creditorsMenu = _webDriver.FindElement(By.Id("Creditors"));
        creditorsMenu.Click();
        var enquiryButton = _webDriver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[1]; 
        enquiryButton.Click();
    }
    
    public void ClickMaintainCreditorButton()
    {
        var maintainButton = _webDriver.FindElement(By.Name("Maintain_Creditor"));
        maintainButton.Click();
    }
    
    public void NavigateToSupplierEnquiry()
    {
        var creditorsMenu = _webDriver.FindElement(By.Id("Stock"));
        creditorsMenu.Click();
        var enquiryButton = _webDriver.FindElements(By.ClassName("OUTER_MENU_ITEM"))[2];
        enquiryButton.Click();
    }
    
    public void ClickMaintainSupplierButton()
    {
        var maintainButton = _webDriver.FindElement(By.Name("Maintain_Supplier"));
        maintainButton.Click();
    }

    public void ClearSearch()
    {
        var clearButton = _webDriver.FindElement(By.Name("clear"));
        clearButton.Click();
    }
    
    public void EnterCreditorCode(string code)
    {
        var inputField = _webDriver.FindElement(By.Name("cAcct__Code"));
        inputField.SendKeys(code);
    }

    public void EnterSupplierCode(string code)
    {
        var inputField = _webDriver.FindElement(By.Name("sSupp__Code"));
        inputField.SendKeys(code);
    }
    
    public void StartSearch()
    {
        var iSearchButton = _webDriver.FindElement(By.Name("Search2"));
        iSearchButton.Click();
    }
    
    public void NavigateBackToOverview()
    {
        _webDriver.Navigate().Back();
        _webDriver.Navigate().Back();
    }
    
    public void NavigateBackToMenu()
    {
        _webDriver.Navigate().Back();
        _webDriver.Navigate().Back();
        _webDriver.Navigate().Back();
    }

    public void ClickSaveButton()
    {
        _webDriver.FindElement(By.Name("save")).Click();
    }

    public IWebElement FindElement(By by)
    {
        return _webDriver.FindElement(by); 
    }

    public ReadOnlyCollection<IWebElement> FindElements(By by)
    {
        return _webDriver.FindElements(by);
    }

    public void SetTimeout(TimeSpan timeSpan)
    {
        _webDriver.Manage().Timeouts().ImplicitWait = timeSpan;
    }

    public void NavigateBack()
    {
        _webDriver.Navigate().Back();
    }
}