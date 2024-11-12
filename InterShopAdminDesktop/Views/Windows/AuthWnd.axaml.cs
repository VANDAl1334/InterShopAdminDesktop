using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using InterShopAdminDesktop.Libs;
using InterShopAdminDesktop.Models;
using InterShopAdminDesktop.ViewModels;
using Newtonsoft.Json;

namespace InterShopAdminDesktop.Views;

/// <summary>
/// Окно входа в систему
/// </summary>
public partial class AuthWnd : Window
{
    private UserViewModel viewModel;
    private List<Setting> settings;
    public string base_url;
    public AuthWnd()
    {
        InitializeComponent();
        viewModel = new UserViewModel(new User() { Login = "Test", Password = "123" });
        settings = LibSettings.GetSettings();
        string url = settings.First(a => a.Name == "base_address").Value;
        if (url != string.Empty && url != null)
        {
            var match = Regex.Match(url, @"^(https?)://([a-zA-Z0-9.-]+)");
            if (match.Success)
            {
                if (match.Groups[1].Value == "http")
                    ProtocolsComBox.SelectedIndex = 0;
                else
                    ProtocolsComBox.SelectedIndex = 1;
                StrConnection.Text = match.Groups[2].Value;
            }
        }
        DataContext = viewModel;
    }
    /// <summary>
    /// Выполняет процедуру авторизации и задаёт токен доступа в случае успеха
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void Btn_Auth(object sender, RoutedEventArgs e)
    {
        MessageBox messageBox;
        string protocol;
        HttpResponseMessage response = new();
        string login = TbLogin.Text;
        string password = LibEncryption.GetHash(TbPassword.Text);
        if (ProtocolsComBox.SelectedIndex != -1 || StrConnection.Text != null)
        {
            ComboBoxItem selectedItem = ProtocolsComBox.SelectedItem as ComboBoxItem;
            protocol = selectedItem.Content.ToString();// Извлекаем Content из выбранного элемента
        }
        else
        {
            messageBox = new MessageBox("Введите все данные для подключения", MessageBoxTypes.Information);
            await messageBox.ShowDialog(this);
            return;
        }
        base_url = protocol + "://" + StrConnection.Text;
        LibWebClient.BaseURL = new Uri(settings.First(s => s.Name == "base_address").Value = protocol + "://" + StrConnection.Text);
        // Попытка получить токен доступа 
        JsonContent data = JsonContent.Create(new { login, password });
        Btn_Auth1.IsEnabled = false;
        response = await LibWebClient.SendAsync(HttpMethod.Post, "api/auth/login", data.Value);
        if (response.StatusCode.ToString() == "NotFound")
        {
            messageBox = new("Неверны логин или пароль", MessageBoxTypes.Error);
            messageBox.ShowDialog(messageBox);
            Btn_Auth1.IsEnabled = true;
        }
        else if (response.StatusCode.ToString() != "OK")
        {
            messageBox = new("Проверьте строку подключения", MessageBoxTypes.Error);
            messageBox.ShowDialog(messageBox);
            Btn_Auth1.IsEnabled = true;
        }
        if (!response.IsSuccessStatusCode)
        {
            messageBox = new("Ошибка: " + response.StatusCode.ToString(), MessageBoxTypes.Error);
            await messageBox.ShowDialog(this);
            return;
        }

        // Содержимое ответа
        dynamic resp = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
        if (resp == null)
            return;
        string token = resp["accessToken"];

        // LibWebClient.Token = token;
        settings.First(p => p.Name == "token").Value = token;
        if (RememberMe.IsChecked == true)
            LibSettings.SaveSettings(settings, LibSettings.FindFile("../", "Settings.json"));
        messageBox = new MessageBox("Вы вошли под пользователем Иды нэхуй", MessageBoxTypes.Information);
        await messageBox.ShowDialog(this);

        // Открытие рабочего окна
        WndMain wnd = new();
        wnd.Show();
        Close();
    }
    // private void Chk_RememberMe(object sender, RoutedEventArgs e)
    // {
    //     // if (RememberMe.IsChecked == true)
    // }
}