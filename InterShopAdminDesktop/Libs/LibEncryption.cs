using System.Text;
using System.Security.Cryptography;

namespace InterShopAdminDesktop.Libs;

/// <summary>
/// Класс для работы с алгоритмами шифрования
/// </summary>
public static class LibEncryption
{
    /// <summary>
    /// Возвращает SHA-256 хэш переданной строки
    /// </summary>
    /// <param name="input">Входная строка</param>
    /// <returns>Хэш</returns>
    public static string GetHash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] inputHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in inputHash)
            {
                stringBuilder.Append(b.ToString("x2"));
            }

            return stringBuilder.ToString();
        }
    }
}