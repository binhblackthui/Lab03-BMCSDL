using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
namespace QLSVProject.Helpers
{
    public static class SecurityHelper
    {
        public static string EncryptWithPublicKey(string plaintext, string publicKeyXml)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKeyXml);
                byte[] data = Encoding.UTF8.GetBytes(plaintext);
                byte[] encrypted = rsa.Encrypt(data, false);
                return Convert.ToBase64String(encrypted);
            }
        }

        public static string HashPasswordSHA1(string password)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static string DecryptWithPrivateKey(string base64Ciphertext, string privateKeyXml)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKeyXml);
                byte[] data = Convert.FromBase64String(base64Ciphertext);
                byte[] decrypted = rsa.Decrypt(data, false);
                return Encoding.UTF8.GetString(decrypted);
            }
        }
        public static void SavePrivateKeyToFile(string privateKeyXml,string manv)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"privateKey{manv}.xml");
            File.WriteAllText(path, privateKeyXml);
        }
        public static string LoadPrivateKeyFromFile(string manv) 
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"privateKey{manv}.xml");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                throw new FileNotFoundException("Private key file not found.", path);
            }

        }
        public static string encryptPrivateWithPassword(string privateKeyXml, string password)
        {
            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKeyXml);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(password.PadRight(32).Substring(0, 32)); // Ensure key is 32 bytes
                aes.GenerateIV();
                using (MemoryStream ms = new MemoryStream())
                {
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(privateKeyBytes, 0, privateKeyBytes.Length);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        public static string DecryptPrivateWithPassword(string encryptedPrivateKey, string password)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedPrivateKey);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(password.PadRight(32).Substring(0, 32)); // Ensure key is 32 bytes
                byte[] iv = new byte[aes.BlockSize / 8];
                Array.Copy(fullCipher, iv, iv.Length);
                aes.IV = iv;
                using (MemoryStream ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
