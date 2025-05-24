using Microsoft.Extensions.Options;
using NoteSystem.BusinessLogic.Extentions;
using NoteSystem.Core.Interfaces;
using System.Text;

namespace NoteSystem.BusinessLogic.Services;
public class XorCryptoService : ICryptoService
{
    private readonly string _password;
    public XorCryptoService(IOptions<EncryptionSettings> encryptionOptions)
    {
        _password = encryptionOptions.Value.Password;
    }
    public string Encrypt(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var keyBytes = Encoding.UTF8.GetBytes(_password);

        var encryptedBytes = new byte[plainBytes.Length];

        for (int i = 0; i < plainBytes.Length; i++)
            encryptedBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length]);

        return Convert.ToBase64String(encryptedBytes);
    }

    public string Decrypt(string encryptedText)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedText);
        var keyBytes = Encoding.UTF8.GetBytes(_password);

        var decryptedBytes = new byte[encryptedBytes.Length];

        for (int i = 0; i < encryptedBytes.Length; i++)
            decryptedBytes[i] = (byte)(encryptedBytes[i] ^ keyBytes[i % keyBytes.Length]);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}