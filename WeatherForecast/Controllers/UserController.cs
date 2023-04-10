using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Services;
using WeatherForecast.Models;

namespace WeatherForecast.Controllers
{
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly DbService _mongoService;
        private readonly HashService _hashService;
        private readonly ElasticService _elasticService;

        public UserController(DbService mongoService, ElasticService elasticService, HashService hashService)
        {
            _mongoService = mongoService;
            _elasticService = elasticService;
            _hashService = hashService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Create/{username}/{password}")]
        public void Create(string username, string password)
        {
            var hashedPassword = _hashService.HashPassword(password);

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
    }
}
