using System;
using Xunit;
using WeSafe.Shared.Extensions;

namespace WeSafe.UnitTests.CryptographyTests
{
    public class CryptographyTests
    {
        private readonly string correctDecryptedString = "rtsp://encr1:encr1@192.168.14.123:80/Streaming/Channels/101";
        private readonly string correctEncryptedString = "EIpkJPk/M1aCfH3sN9Z6wb5Tv2/J85SgvPPZICZv1cB+ElfrgzPR5lH4OUbh5oUvPOz1tFFbbMKhkijBGvKqsL3Clch4QZEOoXyHE1ToVL9UhCdcMpZh7xHFPKkC1uDu0wZmyk/sa3/1sOOroTaETSKkgLPdRX7WYEtKzzayv9A=";

        [Theory]
        [InlineData("123556667657575757575757575757575757575757575757577")]
        [InlineData("1")]
        [InlineData(@"@#%#%#$#^%&&^\\\@////")]
        [InlineData("\"57577")]
        [InlineData("dsfghtyfjujhtgdfsdffsdfs")]
        [InlineData("חותם")]
        public void Encrypt_AnyStringCanBeEncrypted_StringIsEncrypted(string str)
        {
            string encrString = str.Encrypt();

            Assert.NotNull(encrString);
        }

        [Theory]
        [InlineData("       ")]
        [InlineData("")]
        [InlineData(null)]
        public void Encrypt_NullOrWhiteSpaceStringCanNotBeEncrypted_ExceptionIsThrown(string str)
        {
            Assert.Throws<NullReferenceException>(() => str.Encrypt());
        }

        [Fact]
        public void Encrypt_StringCanBeEncrypted_StringIsEncryptedCorrectly()
        {      
            string encrString = correctDecryptedString.Encrypt();

            Assert.Equal(correctEncryptedString, encrString);
        }

        [Fact]
        public void Decrypt_AppropriateEcnryptedStringCanBeDecrypted_StringIsDecryptedCorrectly()
        {
            string decryptedString = correctEncryptedString.Decrypt();

            Assert.Equal(correctDecryptedString, decryptedString);
        }

        [Theory]
        [InlineData("       ")]
        [InlineData("")]
        [InlineData(null)]
        public void Decrypt_NullOrWhiteSpaceStringCanNotBeEncrypted_ExceptionIsThrown(string str)
        {
            Assert.Throws<NullReferenceException>(() => str.Decrypt());
        }

        [Theory]
        [InlineData("123556667657575757575757575757575757575757575757577")]
        [InlineData("1")]
        [InlineData(@"@#%#%#$#^%&&^\\\@////")]
        [InlineData("\"57577")]
        [InlineData("dsfghtyfjujhtgdfsdffsdfs")]
        [InlineData("חותם")]
        public void TryEncrypt_AnyStringCanBeEncrypted_StringIsEncrypted(string str)
        {
            str.TryEncrypt(out string encryptedString);

            Assert.NotNull(encryptedString);
        }

        [Theory]
        [InlineData("123556667657575757575757575757575757575757575757577")]
        [InlineData("1")]
        [InlineData(@"@#%#%#$#^%&&^\\\@////")]
        [InlineData("\"57577")]
        [InlineData("dsfghtyfjujhtgdfsdffsdfs")]
        [InlineData("חותם")]
        public void TryEncrypt_AnyStringCanBeEncrypted_IsSuccesfullS(string str)
        {
            bool isSucceeded = str.TryEncrypt(out string encryptedString);

            Assert.True(isSucceeded);
        }

        [Theory]
        [InlineData("       ")]
        [InlineData("")]
        [InlineData(null)]
        public void TryEncrypt_NullOrWhiteSpaceStringCanNotBeEncrypted_IsNotSuccessful(string str)
        {
            bool isSucceeded = str.TryEncrypt(out string encryptedString);

            Assert.False(isSucceeded);
        }

        [Theory]
        [InlineData("       ")]
        [InlineData("")]
        [InlineData(null)]
        public void TryEncrypt_NullOrWhiteSpaceStringCanNotBeEncrypted_EcryptedStringIsNull(string str)
        {
            str.TryEncrypt(out string encryptedString);

            Assert.Null(encryptedString);
        }

        [Fact]
        public void TryEncrypt_StringCanBeEncrypted_StringIsEncryptedCorrectly()
        {
            correctDecryptedString.TryEncrypt(out string encryptedString);

            Assert.Equal(correctEncryptedString, encryptedString);
        }
          
        [Theory]
        [InlineData("       ")]
        [InlineData("")]
        [InlineData(null)]
        public void TryDecrypt_NullOrWhiteSpaceStringCanNotBeDecrypted_IsNotSuccessful(string str)
        {
            bool isSucceeded = str.TryDecrypt(out string decryptedString);

            Assert.False(isSucceeded);
        }
         
        [Theory]
        [InlineData("       ")]
        [InlineData("")]
        [InlineData(null)]
        public void TryDecrypt_NullOrWhiteSpaceStringCanNotBeDecrypted_DeryptedStringIsNull(string str)
        {
            str.TryDecrypt(out string decryptedString);

            Assert.Null(decryptedString);
        }
          
        [Fact]
        public void TryDecrypt_StringCanBeDecrypted_StringIsDecryptedCorrectly()
        {
            correctEncryptedString.TryDecrypt(out string decryptedString);

            Assert.Equal(correctDecryptedString, decryptedString);
        }

        [Fact]
        public void TryDecrypt_StringCanBeDecrypted_IsSuccessfull()
        {
            bool isSuccessful = correctEncryptedString.TryDecrypt(out string decryptedString);

            Assert.True(isSuccessful);
        }
    }
}
