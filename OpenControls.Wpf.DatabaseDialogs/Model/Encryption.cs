using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace OpenControls.Wpf.DatabaseDialogs.Model
{
    public class Encryption : IEncryption
    {
        private string EncryptionKey = "hr6^gm=£'#$<>";
        private byte[] Bytes = new byte[] { 0x22, 0xEE, 0x6A, 0x8D, 0x7E, 0x92, 0xB9, 0xBC, 0x15, 0xAF, 0xB5, 0xF2, 0x00 };


        public string EncryptDecrypt(bool encrypt, string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            byte[] bytesBuff = null;
            if (encrypt)
            {
                bytesBuff = Encoding.Unicode.GetBytes(input);
            }
            else
            {
                input = input.Replace(" ", "+");
                bytesBuff = Convert.FromBase64String(input);
            }

            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes crypto = new Rfc2898DeriveBytes(EncryptionKey, Bytes);
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (MemoryStream stream = new MemoryStream())
                {
                    ICryptoTransform iCryptoTransform = encrypt ? aes.CreateEncryptor() : aes.CreateDecryptor();
                    using (CryptoStream cStream = new CryptoStream(stream, iCryptoTransform, CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    input = encrypt ? Convert.ToBase64String(stream.ToArray()) : Encoding.Unicode.GetString(stream.ToArray());
                }
            }
            return input;
        }

        public string Encrypt(string rawPassword)
        {
            return EncryptDecrypt(true, rawPassword);
        }

        public string Decrypt(string encryptedPassword)
        {
            return EncryptDecrypt(false, encryptedPassword);
        }
    }
}
