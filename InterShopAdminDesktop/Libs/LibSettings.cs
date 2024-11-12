using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using InterShopAdminDesktop.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace InterShopAdminDesktop.Libs;

/// <summary>
/// Класс для работы с настройками приложения
/// </summary>
public static class LibSettings
{
    private static string _settingsFile = "Settings.json";   // путь к файлу настроек

    /// <summary>
    /// Возвращает список настроек приложения из файла настроек
    /// </summary>
    /// <returns>Список настроек</returns>
    public static List<Setting> GetSettings()
    {
        List<Setting> settings = new();
        string pathSettings = FindFile("../", _settingsFile);
        if (pathSettings == null)
            pathSettings = CreateSettigs();
        string jsonRaw = string.Empty;
        // считывание содержимого файла настроек
        using (StreamReader streamReader = new(pathSettings))
        {
            jsonRaw = streamReader.ReadToEnd();
            streamReader.Close();
        }
        JObject jsonDocument = JObject.Parse(jsonRaw);

        foreach (JProperty group in jsonDocument.Children())
        {
            foreach (JObject obj in group.Children())
            {
                foreach (JProperty property in obj.Properties())
                {
                    settings.Add(new Setting() { Name = property.Name, Value = property.Value.ToString(), Group = group.Name });
                }
            }
        }

        return settings;
    }

    /// <summary>
    /// Записывает переданные настройки приложения в файл настроек
    /// </summary>
    /// <param name="settings">Список настроек</param>
    public static void SaveSettings(List<Setting> settings, string pathSettings)
    {
        settings = settings.OrderBy(x => x.Group).ToList();
        string currentGroup = settings[0].Group;

        JObject doc = new JObject();                // Общий объект, содержащий все настройки
        JObject currentObject = new JObject();      // Объект, хранящий настройки группы
        JProperty jPropertyGroup;                   // Свойство, хранящее название группы

        List<JProperty> properties = new();

        foreach (Setting setting in settings)
        {
            // Если группа настроек изменилась, добавляем её к общему объекту
            if (setting.Group != currentGroup)
            {
                currentObject = new JObject(properties);
                jPropertyGroup = new JProperty(currentGroup) { Value = currentObject };

                doc.Properties().Append(jPropertyGroup);

                currentGroup = setting.Group;
                properties.Clear();
            }

            // Создаём свойство, отображающее значение настройки
            properties.Add(new JProperty(setting.Name) { Value = setting.Value });
        }

        // Добавляем последнюю группу к общему объекту 
        currentObject = new JObject(properties);
        jPropertyGroup = new JProperty(settings[settings.Count - 1].Group) { Value = currentObject };
        doc = new JObject(jPropertyGroup);

        // Запись настроек
        using (StreamWriter streamWriter = new StreamWriter(pathSettings, false, Encoding.UTF8))
        {
            streamWriter.Write(doc.ToString());
            streamWriter.Close();
        }
    }
    public static string FindFile(string startDirectory, string fileName)
    {
        try
        {
            // Ищем файл в текущем каталоге
            foreach (var file in Directory.GetFiles(startDirectory))
            {
                if (Path.GetFileName(file) == fileName)
                {
                    return file; // Возвращаем полный путь найденного файла
                }
            }

            // Рекурсивно обходим все подкаталоги
            foreach (var directory in Directory.GetDirectories(startDirectory))
            {
                var result = FindFile(directory, fileName);
                if (result != null)
                {
                    return result;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Игнорируем каталоги, к которым нет доступа
            Console.WriteLine($"Нет доступа к Директории: {startDirectory}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка доступа к директории {startDirectory}: {ex.Message}");
        }

        return null; // Если файл не найден, возвращаем null
    }
    public static string CreateSettigs()
    {
        var json = new
        {
            connection = new
            {
                base_address = "",
                token = ""
            }
        };

        // Путь к файлу
        string filePath = "Settings.json";

        // Сериализация объекта в JSON и запись в файл
        var options = new JsonSerializerOptions { WriteIndented = true }; // Настройка для форматирования
        string jsonString = System.Text.Json.JsonSerializer.Serialize(json, options);

        File.WriteAllText(filePath, jsonString);

        Console.WriteLine("Конфиг-файл подключения успешно создан.");
        return FindFile("../", filePath);
    }
    public static async Task<bool> Authorize()
    {
        // Получение настроек
        List<Setting> settings = GetSettings();

        // Задание адреса сервера API
        string baseAddress = settings.First(p => p.Name == "base_address").Value;
        if (baseAddress == string.Empty)
            return false;
        LibWebClient.BaseURL = new Uri(baseAddress);

        // Получение токена доступа
        string token = settings.First(p => p.Name == "token").Value;

        if (token == string.Empty)
            return false;

        // Если токен не пустой, устанавливаем его и проверяем его валидность
        LibWebClient.Token = token;
        HttpResponseMessage responseMessage = await LibWebClient.SendAsync(HttpMethod.Get, "api/auth/authorize", null);

        // Если токен валиден, открывается рабочее окно
        if (responseMessage.IsSuccessStatusCode)
            return true;
        else
            return false;
    }
}

/// <summary>
/// Класс, отражающий структуру настройки
/// </summary>
public class Setting
{
    public string Name { get; set; }
    public string Value { get; set; }
    public string Group { get; set; }
}

/// <summary>
/// Группы настроек
/// </summary>
public enum SettingsTypes
{
    Connection,
    Interface
}