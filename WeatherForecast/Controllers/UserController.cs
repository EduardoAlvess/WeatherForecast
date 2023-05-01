using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Services;
using WeatherForecast.Models;
using WeatherForecast.Exceptions;
using System.Net;

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
        [Route("Create")]
        public void Create([FromBody]User user)
        {
            try
            {
                if (_db.GetUserByName(user.Name) is not null)
                    throw new UserAlreadyExistException();

                var hashedPassword = _hashService.HashPassword(user.Password);

                user.Password = hashedPassword;

                _db.CreateUser(user);
            }
            catch(Exception e)
            {
                throw e;
            }
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
