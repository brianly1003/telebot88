using System.Security.Cryptography;
using System.Text;

namespace W88.TeleBot.Utility;

public static class Utility
{
    private const string EncryptionKey = "88048f6aca7941d9a7581c81f06a5a86";

    public static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Generate a random IV
        aes.GenerateIV();
        var iv = aes.IV;

        var encryptor = aes.CreateEncryptor(aes.Key, iv);

        using var memoryStream = new MemoryStream();
        // Prepend the IV to the encrypted data
        memoryStream.Write(iv, 0, iv.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        {
            using (var streamWriter = new StreamWriter(cryptoStream))
            {
                streamWriter.Write(plainText);
            }
        }

        // Return the encrypted data along with the IV as a Base64 string
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // Extract the IV from the beginning of the cipher text
        var iv = new byte[16];
        Array.Copy(fullCipher, 0, iv, 0, iv.Length);

        var decryptor = aes.CreateDecryptor(aes.Key, iv);

        // Extract the actual encrypted data (excluding the IV)
        using var memoryStream = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd();
    }
}