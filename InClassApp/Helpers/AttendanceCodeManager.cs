using InClassApp.Helpers.Interfaces;
using InClassApp.Models.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TwoFactorAuthNet;

namespace InClassApp.Helpers
{
    public class AttendanceCodeManager : IAttendanceCodeManager
    {
        public string CODE_GENERATION_SECRET { get; set; }
        public string CODE_ENCRYPTION_KEY { get; set; }

        public AttendanceCodeManager(IConfiguration configuration)
        {
            CODE_GENERATION_SECRET = configuration["AttendanceCodeSecrets:CodeGenerationSecret"] ?? throw new ConfigurationErrorsException("CodeGenerationSecret");
            CODE_ENCRYPTION_KEY = configuration["AttendanceCodeSecrets:CodeEncryptionKey"] ?? throw new ConfigurationErrorsException("CodeEncryptionKey");
        }

        public string CreateAttendanceCode()
        {
            var tfa = new TwoFactorAuth();
            var code = tfa.GetCode(CODE_GENERATION_SECRET);
            return code;
        }

        public void EncryptMeeting(Meeting meetingToEncrypt, string generatedCode)
        {
            if (generatedCode == null)
            {
                return;
            }
            Aes cipher = CreateCipher();

            meetingToEncrypt.LastlyGeneratedCodeIV = Convert.ToBase64String(cipher.IV);

            ICryptoTransform cryptoTransform = cipher.CreateEncryptor();
            byte[] codeBytes = Encoding.UTF8.GetBytes(generatedCode);
            byte[] cipherText = cryptoTransform.TransformFinalBlock(codeBytes, 0, codeBytes.Length);

            meetingToEncrypt.LastlyGeneratedCheckCode = Convert.ToBase64String(cipherText);
        }

        public string GetDecryptedCode(string codeEncrypted, string IV)
        {
            if(codeEncrypted == null)
            {
                return null;
            }
            Aes cipher = CreateCipher();
            cipher.IV = Convert.FromBase64String(IV);

            ICryptoTransform cryptoTransform = cipher.CreateDecryptor();
            byte[] cipherText = Convert.FromBase64String(codeEncrypted);
            byte[] originalCodeBytes = cryptoTransform.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(originalCodeBytes);
        }

        private Aes CreateCipher()
        {
            Aes cipher = Aes.Create();
            cipher.Padding = PaddingMode.ISO10126;

            cipher.Key = StringToByteArray(CODE_ENCRYPTION_KEY);

            return cipher;
        }

        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}
