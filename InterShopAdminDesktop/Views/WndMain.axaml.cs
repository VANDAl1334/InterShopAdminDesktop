using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InterShopAdminDesktop.Libs;
using InterShopAdminDesktop.Models;
using InterShopAdminDesktop.ViewModels;
using Newtonsoft.Json;

namespace InterShopAdminDesktop.Views;

/// <summary>
/// Главное окно
/// </summary>
public partial class WndMain : Window
{
    public WndMain()
    {
        InitializeComponent();
    }
}