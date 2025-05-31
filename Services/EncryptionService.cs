using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using EmployeeManagementApi.Settings;

namespace EmployeeManagementApi.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService(IOptions<EncryptionSettings> options)
        {
            _key = Encoding.UTF8.GetBytes(options.Value.Key);
            _iv = Encoding.UTF8.GetBytes(options.Value.IV);
        }

        public byte[] Encrypt(string plainText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);

            var encrypted = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            return encrypted;
        }

        public string Decrypt(byte[] cipherText)
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}