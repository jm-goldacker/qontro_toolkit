using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Qontro.Toolkit.Logic.WebDriver;

public static class SeleniumWebDriver
{
    private static readonly Lazy<IWebDriver> LazyInstance = new(() =>
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.SuppressInitialDiagnosticInformation = true;
        var options = new ChromeOptions();
        options.AddArgument("--no-sandbox");
        return new ChromeDriver(service, options);
    });
    
    public static IWebDriver Instance => LazyInstance.Value;
}