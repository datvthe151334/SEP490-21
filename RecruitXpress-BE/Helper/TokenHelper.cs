using System.Security.Cryptography;

namespace RecruitXpress_BE.Helper
{
    public class TokenHelper
    {
        public static string GenerateRandomToken(int length)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var charactersLength = validCharacters.Length;
                var randomChars = new char[length];

                for (int i = 0; i < length; i++)
                {
                    byte[] randomBytes = new byte[1];
                    rng.GetBytes(randomBytes);
                    char randomChar = validCharacters[randomBytes[0] % charactersLength];
                    randomChars[i] = randomChar;
                }

                return new string(randomChars);
            }
        }
        public static string GenerateNumericToken(int length)
        {
            Random random = new Random();
            const string characters = "0123456789";
            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string GenerateRandomToken()
        {
            var random = new Byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
    }
}
