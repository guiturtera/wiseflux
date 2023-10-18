using System.Security.Cryptography;

namespace Wiseflux.Security
{
    public class UserSecurity
    {
        public bool CheckPassword(string password, string hashPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashPassword);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;

            return true;
        }

        public string EncryptPassword(string password)
        {
            // Hash + salt for encrypt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] combinedHash = new byte[36];

            Array.Copy(salt, 0, combinedHash, 0, 16);
            Array.Copy(hash, 0, combinedHash, 16, 20);

            return Convert.ToBase64String(combinedHash);
        }

    }
}
