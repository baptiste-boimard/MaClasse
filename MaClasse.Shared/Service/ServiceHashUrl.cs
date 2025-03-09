using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MaClasse.Shared.Models;
using Microsoft.Extensions.Configuration;

namespace MaClasse.Shared.Service;

public class ServiceHashUrl
{
    private readonly IConfiguration _configuration;

    public ServiceHashUrl(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string EncryptErrorOAuth(ErrorOAuth errorOAuth)
    {
        //* Sérialisation en JSON et conversion en byte[]
        string json = JsonSerializer.Serialize(errorOAuth);
        var byteJson = Encoding.UTF8.GetBytes(json);
        
        //* Récupération des clés de cryptage
        var keyString  = _configuration["Secrets:EncryptionKey"];
        var ivString   = _configuration["Secrets:EncryptionIV"];
        byte[] key = Encoding.UTF8.GetBytes(keyString);
        byte[] iv = Encoding.UTF8.GetBytes(ivString);
        
        //* Chiffrement
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(byteJson, 0, byteJson.Length);
                    cryptoStream.FlushFinalBlock();
                }
                // Retourne le contenu chiffré en Base64
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
    
    public ErrorOAuth DecryptErrorOAuth(string encryptedBase64)
    {
        byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);
            
        var keyString = _configuration["Secrets:EncryptionKey"];
        var ivString = _configuration["Secrets:EncryptionIV"];
            
        byte[] key = Encoding.UTF8.GetBytes(keyString);
        byte[] iv = Encoding.UTF8.GetBytes(ivString);
            
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = iv;
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(cipherBytes))
            using (var cryptoStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cryptoStream, Encoding.UTF8))
            {
                string json = reader.ReadToEnd();
                return JsonSerializer.Deserialize<ErrorOAuth>(json);
            }
        }
    }
}