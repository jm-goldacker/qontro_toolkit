using Microsoft.Extensions.DependencyInjection;
using Qontro.Toolkit.Logic.Export;
using Qontro.Toolkit.Logic.Import;
using Qontro.Toolkit.Logic.WebDriver;
using Qontro.Toolkit.Ui.ViewModels;

namespace Qontro.Toolkit.Ui;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainWindowViewModel>();

        collection.AddSingleton<ISeleniumWebDriver, SeleniumWebDriver>();
        collection.AddSingleton<ICreditorExport, CreditorExport>();
        collection.AddSingleton<ISupplierExport, SupplierExport>();
        collection.AddSingleton<ICreditorImport, CreditorImport>();
        collection.AddSingleton<ISupplierImport, SupplierImport>();
    }
}