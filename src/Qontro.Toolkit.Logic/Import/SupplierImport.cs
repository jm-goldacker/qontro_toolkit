using OpenQA.Selenium;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Import;

public class SupplierImport(ISeleniumWebDriver webDriver) : AccountImport(webDriver), ISupplierImport
{
    protected override void NavigateToEnquiry()
    {
        WebDriver.NavigateToSupplierEnquiry();
    }

    protected override int GetCsvFieldOffset()
    {
        return 2;
    }

    protected override void EnterCode(string code)
    {
        WebDriver.EnterSupplierCode(code);
    }

    protected override void ClickMaintainButton()
    {
        WebDriver.ClickMaintainSupplierButton();
    }

    protected override void ClickMainMenuButton()
    {
        WebDriver.FindElement(By.Id("menu-stock-home")).Click();
    }
}