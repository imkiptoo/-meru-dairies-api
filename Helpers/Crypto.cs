using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace API.Helpers
{
    public static class Crypto
    {
        static readonly byte[] Bytes = Encoding.UTF8.GetBytes("BfKzPbBC4Jg57ZqP");
        static readonly byte[] Iv = Encoding.UTF8.GetBytes("BfKzPbBC4Jg57ZqP");
        
        public static string Decrypt(string cipherText)
        {
            var encrypted = Convert.FromBase64String(cipherText);
            var decryptedFromJavascript = DecryptStringFromBytes(encrypted, Bytes, Iv);
            return decryptedFromJavascript;
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText is not { Length: > 0 })
            {
                throw new ArgumentNullException(nameof(cipherText));
            }

            if (key is not { Length: > 0 })
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (iv is not { Length: > 0 })
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using var rijAlg = new RijndaelManaged();
            //Settings
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;

            rijAlg.Key = key;
            rijAlg.IV = iv;

            // Create a decrypt to perform the stream transform.
            var decrypt = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

            try
            {
                // Create the streams used for decryption.
                using var msDecrypt = new MemoryStream(cipherText);
                using var csDecrypt = new CryptoStream(msDecrypt, decrypt, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt);
                // Read the decrypted bytes from the decrypting stream
                // and place them in a string.
                plaintext = srDecrypt.ReadToEnd();
            }
            catch
            {
                plaintext = "keyError";
            }

            return plaintext;
        }

        public static string Encrypt(string plainText)
        {
            var encryptFromJavascript = EncryptStringToBytes(plainText, Bytes, Iv);
            return Convert.ToBase64String(encryptFromJavascript);
        }

        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] theIv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException(nameof(plainText));
            }

            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (theIv == null || theIv.Length <= 0)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Create a RijndaelManaged object
            // with the specified key and IV.
            using var rijAlg = new RijndaelManaged();
            rijAlg.Mode = CipherMode.CBC;
            rijAlg.Padding = PaddingMode.PKCS7;
            rijAlg.FeedbackSize = 128;

            rijAlg.Key = key;
            rijAlg.IV = theIv;

            // Create a encryptor to perform the stream transform.
            var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

            // Create the streams used for encryption.
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                //Write all data to the stream.
                swEncrypt.Write(plainText);
            }

            var encrypted = msEncrypt.ToArray();

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}