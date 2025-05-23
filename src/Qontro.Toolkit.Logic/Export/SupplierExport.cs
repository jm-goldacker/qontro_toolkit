﻿using System.Collections.ObjectModel;
using OpenQA.Selenium;
using Qontro.Toolkit.Logic.WebDriver;

namespace Qontro.Toolkit.Logic.Export;

public class SupplierExport(ISeleniumWebDriver webDriver) : AccountExport(webDriver), ISupplierExport
{
    protected override void NavigateToAccount()
    {
        WebDriver.NavigateToSupplierEnquiry();
    }

    protected override void InitExportFields()
    {
        ExportRow = new List<string>();
        ExportFieldNames = ["", ""];
        Headers = new List<string>();
    }

    protected override void ExportMetadata(ReadOnlyCollection<IWebElement> columns)
    {
        Headers.Add("Linked Stock");
        ExportRow.Add(columns.Last().Text);
    }

    protected override bool ExportLastUpdated()
    {
        return false;
    }
    
    protected override void ClickMaintainButton()
    {
        WebDriver.ClickMaintainSupplierButton();
    }
}