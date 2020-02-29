using System;
using System.Security.Cryptography;
using System.Text;

namespace GigHub.Security
{
    public class TripleDes
    {
        public static string Encrypt(string source, string key)
        {
            var desCryptoProvider = new TripleDESCryptoServiceProvider();
            var hashMd5Provider = new MD5CryptoServiceProvider();
            var byteHash = hashMd5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            var byteBuff = Encoding.UTF8.GetBytes(source);
            var encoded = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }

        public static string Decrypt(string encodedText, string key)
        {
            var plaintext = string.Empty;
            var desCryptoProvider = new TripleDESCryptoServiceProvider();
            var hashMd5Provider = new MD5CryptoServiceProvider();
            var byteHash = hashMd5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            var byteBuff = Convert.FromBase64String(encodedText);
            try
            {
                plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            }

            catch (Exception e)
            {
                plaintext = e.Message;
            }

            return plaintext;
        }
    }
}
