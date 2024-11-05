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
/// Окно входа в систему
/// </summary>
public partial class AuthWnd : Window
{
    private UserViewModel viewModel;

    public AuthWnd()
    {
        viewModel = new UserViewModel(new User() { Login = "Test", Password = "123" });

        InitializeComponent();
        DataContext = viewModel;
    }

    public async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Получение настроек
        List<Setting> settings = LibSettings.GetSettings();

        // Задание адреса сервера API
        string baseAddress = settings.First(p => p.Name == "base_address").Value;
        LibWebClient.BaseURL = new Uri(baseAddress);

        // Получение токена доступа
        string token = settings.First(p => p.Name == "token").Value;

        if (token == string.Empty)
            return;

        // Если токен не пустой, устанавливаем его и проверяем его валидность
        LibWebClient.Token = token;
        HttpResponseMessage responseMessage = await LibWebClient.SendAsync(HttpMethod.Get, "api/auth/authorize", null);

        // Если токен валиден, открывается рабочее окно
        if (responseMessage.IsSuccessStatusCode)
        {
            WndMain wnd = new WndMain();
            wnd.Show();
            Close();
        }
        else
        {
            MessageBox messageBox = new MessageBox("Не удалось войти. Попробуйте ввести логин и пароль", MessageBoxTypes.Error);
            await messageBox.ShowDialog(this);
        }
    }

    /// <summary>
    /// Выполняет процедуру авторизации и задаёт токен доступа в случае успеха
    /// </summary>
    private async void auth()
    {
        string login = TbLogin.Text;
        string password = LibEncryption.GetHash(TbPassword.Text);


        MessageBox messageBox;

        // Попытка получить токен доступа 
        HttpResponseMessage response = await LibWebClient.SendAsync(HttpMethod.Post, "api/auth/login", JsonContent.Create(new { login, password }));

        if (!response.IsSuccessStatusCode)
        {
            messageBox = new MessageBox("Не удалось войти, попробуйте ещё раз", MessageBoxTypes.Error);
            await messageBox.ShowDialog(this);

            return;
        }

        // Содержимое ответа
        dynamic resp = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

        // Устанавливаем и сохраняем токен доступа 
        string token = resp["accessToken"];

        LibWebClient.Token = token;

        List<Setting> settings = LibSettings.GetSettings();
        settings.First(p => p.Name == "token").Value = token;
        LibSettings.SaveSettings(settings);

        messageBox = new MessageBox("Вы вошли под пользователем Иды нэхуй", MessageBoxTypes.Information);
        await messageBox.ShowDialog(this);

        // Открытие рабочего окна
        WndMain wnd = new WndMain();
        wnd.Show();
        Close();
    }

    /// <summary>
    /// Обработчик нажатия кнопки "Войти"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public async void Button_Click(object sender, RoutedEventArgs e)
    {
        auth();
    }
}