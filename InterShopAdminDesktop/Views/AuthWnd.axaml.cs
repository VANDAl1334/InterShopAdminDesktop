using Avalonia.Controls;
using Avalonia.Interactivity;
using InterShopAdminDesktop.Models;
using InterShopAdminDesktop.ViewModels;

namespace InterShopAdminDesktop.Views;

public partial class AuthWnd : Window
{
    private UserViewModel viewModel;
    public AuthWnd()
    {
        viewModel = new UserViewModel(new User() { Login="Test", Password="123"});
        
        InitializeComponent();
        DataContext = viewModel;
    }

    public void Button_Click(object sender, RoutedEventArgs e)
    {

    }
}