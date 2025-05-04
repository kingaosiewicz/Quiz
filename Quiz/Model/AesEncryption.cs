using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.Model
{
    public static class AesEncryption
    {
        private static readonly int KeySize = 256; // bits
        private static readonly int IvSize = 16;   // bytes

        public static byte[] Encrypt(string plainText, string password)
        {
            byte[] salt = GenerateRandomBytes(16);
            using var aes = Aes.Create();
            using var key = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            aes.Key = key.GetBytes(KeySize / 8);
            aes.IV = GenerateRandomBytes(IvSize);
            aes.Mode = CipherMode.CBC;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            ms.Write(salt);
            ms.Write(aes.IV);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(plainText);

            return ms.ToArray();
        }

        public static string Decrypt(byte[] encryptedData, string password)
        {
            byte[] salt = new byte[16];
            byte[] iv = new byte[IvSize];
            Array.Copy(encryptedData, 0, salt, 0, salt.Length);
            Array.Copy(encryptedData, salt.Length, iv, 0, iv.Length);

            using var aes = Aes.Create();
            using var key = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            aes.Key = key.GetBytes(KeySize / 8);
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(encryptedData, salt.Length + iv.Length, encryptedData.Length - salt.Length - iv.Length);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }

        private static byte[] GenerateRandomBytes(int count)
        {
            byte[] bytes = new byte[count];
            RandomNumberGenerator.Fill(bytes);
            return bytes;
        }
    }
}
