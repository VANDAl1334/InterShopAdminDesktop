using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using InterShopAdminDesktop.Libs;
using InterShopAdminDesktop.Models;
using InterShopAdminDesktop.ViewModels;
using InterShopAdminDesktop.Views.Pages;
using System.Text.Json;
// using Avalonia

namespace InterShopAdminDesktop.Views;

/// <summary>
/// Главное окно
/// </summary>
public partial class WndMain : Window
{
    MessageBox messagebox;
    DispatcherTimer timer;
    private bool hidden = true;
    public WndMain()
    {
        InitializeComponent();
        timer = new();
        timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        timer.Tick += timer_Tick;
        Btn_Menu.AddHandler(PointerPressedEvent, Menu_Pressed, handledEventsToo: true);
        Products.AddHandler(PointerPressedEvent, ListBoxProducts_Pressed, handledEventsToo: true);
        Category.AddHandler(PointerPressedEvent, ListBoxCategory_Pressed, handledEventsToo: true);
        Discounts.AddHandler(PointerPressedEvent, ListBoxDiscounts_Pressed, handledEventsToo: true);
        Users.AddHandler(PointerPressedEvent, ListBoxUsers_Pressed, handledEventsToo: true);
        Statistics.AddHandler(PointerPressedEvent, ListBoxStatistics_Pressed, handledEventsToo: true);
        Profile.AddHandler(PointerPressedEvent, ListBoxProfile_Pressed, handledEventsToo: true);
        Settings.AddHandler(PointerPressedEvent, ListBoxSettings_Pressed, handledEventsToo: true);
        Exit.AddHandler(PointerPressedEvent, ListBoxExit_Pressed, handledEventsToo: true);
    }
    private void timer_Tick(object sender, EventArgs e)
    {
        if (hidden)
        {
            Menu.Width += 20;
            if (Menu.Width >= 180)
            {
                timer.Stop();
                hidden = false;
            }
        }
        else
        {
            Menu.Width -= 20;
            if (Menu.Width < 58)
            {
                timer.Stop();
                hidden = true;
            }
        }
    }
    private void Menu_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            timer.Start();
    }
    private void ListBoxProducts_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            MainFrame.Content = new PgProducts();
    }
    private void ListBoxCategory_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            MainFrame.Content = new PgCategory();
    }
    private void ListBoxDiscounts_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            MainFrame.Content = new PgDiscounts();
    }
    private void ListBoxUsers_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            MainFrame.Content = new PgUsers();
    }
    private void ListBoxStatistics_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            MainFrame.Content = new PgStatistics();
    }
    private void ListBoxProfile_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            MainFrame.Content = new PgProfile();
    }
    private void ListBoxSettings_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            MainFrame.Content = new PgSettings();
    }
    private async void ListBoxExit_Pressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        if (point.Properties.IsLeftButtonPressed)
            messagebox = new("Вы точно хотите выйти?", MessageBoxTypes.Question);
        bool res = await messagebox.ShowDialog(this);
        if (res)
        {
            List<Setting> settings = LibSettings.GetSettings();
            settings.First(s => s.Name == "token").Value = string.Empty;
            if (settings != null)
                LibSettings.SaveSettings(settings, LibSettings.FindFile("../", "Settings.json"));
            AuthWnd authWnd = new();
            authWnd.Show();
            Close();
        }

    }
}