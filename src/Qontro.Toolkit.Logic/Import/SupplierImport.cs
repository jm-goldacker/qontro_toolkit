using OpenQA.Selenium;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Import;

public class SupplierImport : AccountImport
{
    protected override void NavigateToEnquiry()
    {
        SeleniumWebDriver.Instance.NavigateToSupplierEnquiry();
    }

    protected override int GetCsvFieldOffset()
    {
        return 2;
    }

    protected override void EnterCode(string code)
    {
        SeleniumWebDriver.Instance.EnterSupplierCode(code);
    }

    protected override void ClickMaintainButton()
    {
        SeleniumWebDriver.Instance.ClickMaintainSupplierButton();
    }

    protected override void ClickMainMenuButton()
    {
        SeleniumWebDriver.Instance.FindElement(By.Id("menu-stock-home")).Click();
    }
}