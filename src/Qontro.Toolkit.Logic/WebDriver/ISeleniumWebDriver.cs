using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Qontro.Toolkit.Logic.WebDriver;

public interface ISeleniumWebDriver
{
    bool Login(string url, string username, string password);
    void NavigateToCreditorEnquiry();
    void ClickMaintainCreditorButton();
    void NavigateToSupplierEnquiry();
    void ClickMaintainSupplierButton();
    void ClearSearch();
    void EnterCreditorCode(string code);
    void EnterSupplierCode(string code);
    void StartSearch();
    void NavigateBackToOverview();
    void NavigateBackToMenu();
    void ClickSaveButton();
    IWebElement FindElement(By by);
    ReadOnlyCollection<IWebElement> FindElements(By by);
    void SetTimeout(TimeSpan timeSpan);
    void NavigateBack();
}