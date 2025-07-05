using System;
using System.Security.Cryptography;
using System.Text;

namespace FUNSAR.HerramientasComunes
{
    public class Encrypt
    {
        public string sha256(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("El input no puede ser nulo o vacío", nameof(input));
            }

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);

                byte[] hashBytes = sha256.ComputeHash(bytes);

                StringBuilder hashStringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashStringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return hashStringBuilder.ToString();
            }
        }
    }
}
