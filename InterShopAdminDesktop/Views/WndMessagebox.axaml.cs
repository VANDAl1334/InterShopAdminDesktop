
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using InterShopAdminDesktop.Models;
using InterShopAdminDesktop.ViewModels;
using Newtonsoft.Json;

namespace InterShopAdminDesktop.Views;

/// <summary>
/// Окно для показа сообщений пользователю
/// </summary>
public partial class MessageBox : Window
{
    private bool result;    // Выбор пользователя (Ok / Cancel) 

    public MessageBox(string content, MessageBoxTypes type)
    {
        InitializeComponent();

        TblContent.Text = content;

        switch (type)
        {
            case MessageBoxTypes.Information:
                ImgMessageType.Source = new Bitmap(AssetLoader.Open(new Uri("avares://InterShopAdminDesktop/Assets/InterShopDesktop_Icons/Information.png")));
                break;
            case MessageBoxTypes.Question:
                ImgMessageType.Source = new Bitmap(AssetLoader.Open(new Uri("avares://InterShopAdminDesktop/Assets/InterShopDesktop_Icons/Question.png")));
                Grid.SetColumnSpan(BtnOk, 2);
                BtnCancel.IsVisible = true;
                break;
            case MessageBoxTypes.Error:
                ImgMessageType.Source = new Bitmap(AssetLoader.Open(new Uri("avares://InterShopAdminDesktop/Assets/InterShopDesktop_Icons/Error.png")));
                break;
        }
    }

    /// <summary>
    /// Вызывает окно в модальном режиме. По закрытию возвращает ответ пользователя 
    /// </summary>
    /// <param name="owner"></param>
    /// <returns></returns>
    public new async Task<bool> ShowDialog(Window owner)
    {
        await base.ShowDialog(owner);

        return result;
    }

    /// <summary>
    /// Обработчик нажатия кнопок
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void Btn_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button)
            return;

        result = (sender as Button).Name == "BtnOk" ? true : false;
        Close();
    }
}

/// <summary>
/// Типы сообщений пользователю
/// </summary>
public enum MessageBoxTypes
{
    Error,
    Information,
    Question
}