using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ApiOAuthEmpleados.Helpers
{
    public class HelperCifrado
    {
        
        //private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012"); // 32 chars
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456"); // 16 chars

        public static string EncryptString(string texto, byte[] clave)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = clave;
                aes.IV = IV;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using(MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(texto);
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        public static string DecryptString(string textoCifrado, byte[] clave)
        {
            using(Aes aes = Aes.Create())
            {
                aes.Key = clave;
                aes.IV = IV;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(textoCifrado)))
                {
                    using(CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
