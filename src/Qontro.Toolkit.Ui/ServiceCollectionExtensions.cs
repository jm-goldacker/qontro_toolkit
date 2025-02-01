using Microsoft.Extensions.DependencyInjection;
using Qontro.Toolkit.Ui.ViewModels;

namespace Qontro.Toolkit.Ui;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainWindowViewModel>();
    }
}