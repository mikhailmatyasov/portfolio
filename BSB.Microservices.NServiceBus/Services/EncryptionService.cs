using NServiceBus.Pipeline;
using IEncryptionService = NServiceBus.Encryption.MessageProperty.IEncryptionService;
using EncryptedValue = NServiceBus.Encryption.MessageProperty.EncryptedValue;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;
using System.Linq;

namespace BSB.Microservices.NServiceBus
{
    public class EncryptionService : IEncryptionService
    {
        public string Decrypt(EncryptedValue encryptedValue, IIncomingLogicalMessageContext context)
        {
            if (string.IsNullOrWhiteSpace(encryptedValue?.Base64Iv) || string.IsNullOrWhiteSpace(encryptedValue?.EncryptedBase64Value))
            {
                return encryptedValue?.EncryptedBase64Value;
            }

            string plaintext = null;

            using (var aes = new AesManaged
            {
                Key = GetKey(context.MessageId),
                IV = Convert.FromBase64String(encryptedValue.Base64Iv)
            })
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var memStream = new MemoryStream(Convert.FromBase64String(encryptedValue.EncryptedBase64Value)))
            using (var cryptoStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Read))
            using (var streamReader = new StreamReader(cryptoStream))
            {
                plaintext = streamReader.ReadToEnd();
            }

            return plaintext;
        }

        public EncryptedValue Encrypt(string value, IOutgoingLogicalMessageContext context)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new EncryptedValue
                {
                    EncryptedBase64Value = value
                };
            };

            EncryptedValue encrypted;

            using (var aes = new AesManaged
            {
                Key = GetKey(context.MessageId)
            })
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var memStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
            {
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(value);
                }

                encrypted = new EncryptedValue
                {
                    Base64Iv = Convert.ToBase64String(aes.IV),
                    EncryptedBase64Value = Convert.ToBase64String(memStream.ToArray())
                };
            }

            return encrypted;
        }

        private byte[] GetKey(string messageId)
        {
#if NET452
            var bytes = Encoding.UTF8.GetBytes(messageId);
#else
            Span<byte> bytes = Encoding.UTF8.GetBytes(messageId);
#endif

            if (bytes.Length > 32)
            {
#if NET452
                return bytes.Take(32).ToArray();
#else
                return bytes.Slice(0, 32).ToArray();
#endif
            }
            else if (bytes.Length < 32)
            {
                var newArray = new byte[32];
                var startAt = newArray.Length - bytes.Length;

                Array.Copy(bytes.ToArray(), 0, newArray, startAt, bytes.Length);

                return newArray;
            }

            return bytes.ToArray();
        }
    }
}
