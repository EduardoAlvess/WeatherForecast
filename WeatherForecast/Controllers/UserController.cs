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
        private readonly IDbService _db;
        private readonly HashService _hashService;
        private readonly ILogService _logger;

        public UserController(IDbService db, ILogService logService, HashService hashService)
        {
            _db = db;
            _logger = logService;
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

            _db.CreateUser(user);
        }


        [HttpGet]
        [Authorize]
        [Route("GetUserLogs")]
        public List<Log> GetUserLogs()
        {
            return _logger.GetUserLogs();
        }
    }
}
