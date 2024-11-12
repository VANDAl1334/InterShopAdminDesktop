using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using InterShopAdminDesktop.Libs;
using InterShopAdminDesktop.Views;

namespace InterShopAdminDesktop;

public partial class App : Application
{
    public static Window CurrentWindow;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        bool result = await LibSettings.Authorize();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && result)
        {            
            desktop.MainWindow = new WndMain();
            CurrentWindow = desktop.MainWindow;
            desktop.MainWindow.Show();
        }
        else if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopq)
        {
            desktopq.MainWindow = new AuthWnd();
            CurrentWindow = desktopq.MainWindow;
            desktopq.MainWindow.Show();
        }
        base.OnFrameworkInitializationCompleted();
    }
}