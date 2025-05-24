namespace NoteSystem.Core.Interfaces;

public interface ICryptoService
{
    string Decrypt(string encryptedText);
    string Encrypt(string plainText);
}