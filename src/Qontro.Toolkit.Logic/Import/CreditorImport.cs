using OpenQA.Selenium;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Import;

public class CreditorImport(ISeleniumWebDriver webDriver) : AccountImport(webDriver), ICreditorImport
{
    protected override void NavigateToEnquiry()
    {
        WebDriver.NavigateToCreditorEnquiry();
    }

    protected override int GetCsvFieldOffset()
    {
        return 5;
    }

    protected override void EnterCode(string code)
    {
        WebDriver.EnterCreditorCode(code);
    }

    protected override void ClickMaintainButton()
    {
        WebDriver.ClickMaintainCreditorButton();
    }

    protected override void ClickMainMenuButton()
    {
        WebDriver.FindElement(By.Id("menu-creditor-home")).Click();
    }
}