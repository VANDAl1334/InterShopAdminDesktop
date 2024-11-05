using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InterShopAdminDesktop.Libs;

/// <summary>
/// Класс для работы с настройками приложения
/// </summary>
public static class LibSettings
{
    private const string _settingsFile = "Settings.json";   // путь к файлу настроек

    /// <summary>
    /// Возвращает список настроек приложения из файла настроек
    /// </summary>
    /// <returns>Список настроек</returns>
    public static List<Setting> GetSettings()
    {
        List<Setting> settings = new List<Setting>();

        string jsonRaw = string.Empty;
        // считывание содержимого файла настроек
        using (StreamReader streamReader = new StreamReader(_settingsFile))
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
    public static void SaveSettings(List<Setting> settings)
    {
        settings = settings.OrderBy(x => x.Group).ToList();
        string currentGroup = settings[0].Group;

        JObject doc = new JObject();                // Общий объект, содержащий все настройки
        JObject currentObject = new JObject();      // Объект, хранящий настройки группы
        JProperty jPropertyGroup;                   // Свойство, хранящее название группы

        List<JProperty> properties = new List<JProperty>();

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
        using (StreamWriter streamWriter = new StreamWriter("Settings.json", false, Encoding.UTF8))
        {
            streamWriter.Write(doc.ToString());
            streamWriter.Close();
        }
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