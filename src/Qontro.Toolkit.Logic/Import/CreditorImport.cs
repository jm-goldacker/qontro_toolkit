﻿using OpenQA.Selenium;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Import;

public class CreditorImport : AccountImport
{
    protected override void NavigateToEnquiry()
    {
        SeleniumWebDriver.Instance.NavigateToCreditorEnquiry();
    }

    protected override int GetCsvFieldOffset()
    {
        return 5;
    }

    protected override void EnterCode(string code)
    {
        SeleniumWebDriver.Instance.EnterCreditorCode(code);
    }

    protected override void ClickMaintainButton()
    {
        SeleniumWebDriver.Instance.ClickMaintainCreditorButton();
    }

    protected override void ClickMainMenuButton()
    {
        SeleniumWebDriver.Instance.FindElement(By.Id("menu-creditor-home")).Click();
    }
}