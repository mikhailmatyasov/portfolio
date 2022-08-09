using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace WeSafe.Shared.Extensions
{
    public static class StringCryptographylExtension
    {
        //[LS] Do not change it. If the key changes all encrypted records won't be able to be decrypted.
        private static readonly string _keyString = "E546C8DF278CD5931069B522E695D4F2";

        public static string Encrypt(this string encryptedText)
        {
            if (string.IsNullOrWhiteSpace(encryptedText))
                throw new NullReferenceException(nameof(encryptedText));

            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptedText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_keyString, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptedText = Convert.ToBase64String(ms.ToArray());
                }
            }

            return encryptedText;
        }

        public static string Decrypt(this string decryptedText)
        {
            if (string.IsNullOrWhiteSpace(decryptedText))
                throw new NullReferenceException(nameof(decryptedText));

            decryptedText = decryptedText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(decryptedText);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_keyString, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    decryptedText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }

            return decryptedText;
        }

        public static bool TryEncrypt(this string unencryptedString, out string encryptedString)
        {
            try
            {
                encryptedString = unencryptedString.Encrypt();

                return true;
            }            
            catch
            {
                encryptedString = null;

                return false;
            }         
        }

        public static bool TryDecrypt(this string encryptedString, out string decryptedString)
        {
            try
            {
                decryptedString = encryptedString.Decrypt();

                return true;
            }
            catch
            {
                decryptedString = null;

                return false;
            }
        }
    }
}
