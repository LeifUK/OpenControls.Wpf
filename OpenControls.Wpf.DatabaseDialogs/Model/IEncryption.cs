namespace OpenControls.Wpf.DatabaseDialogs.Model
{
    public interface IEncryption
    {
        string Encrypt(string rawPassword);
        string Decrypt(string encryptedPassword);
    }
}
