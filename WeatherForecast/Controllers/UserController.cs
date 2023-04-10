using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Services;
using WeatherForecast.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace WeatherForecast.Controllers
{
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly DbService _mongoService;
        private readonly ElasticService _elasticService;

        public UserController(DbService mongoService, ElasticService elasticService)
        {
            _mongoService = mongoService;
            _elasticService = elasticService;
        }

        [HttpPost]
        [Route("Create/{username}/{password}")]
        public void Create(string username, string password)
        {
            var hashedPassword = HashPassword(password);

            var user = new User
            {
                Name = username,
                Password = hashedPassword
            };

            _mongoService.CreateUser(user);
        }


        [HttpGet]
        [Authorize]
        [Route("GetUserLogs")]
        public List<Log> GetUserLogs()
        {
            return _elasticService.GetUserLogs();
        }

        private string HashPassword(string password)
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
