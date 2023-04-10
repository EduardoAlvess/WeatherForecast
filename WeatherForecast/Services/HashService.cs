using System.Security.Cryptography;
using System.Text;

namespace WeatherForecast.Services
{
    public class HashService
    {
        public string HashPassword(string password)
        {
            using (var md5Hash = MD5.Create())
            {
                var sourceBytes = Encoding.UTF8.GetBytes(password);

                var hashBytes = md5Hash.ComputeHash(sourceBytes);

                var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

                return hash;
            }
        }
    }
}
