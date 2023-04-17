using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using WeatherForecast.Services;
using System.Security.Claims;
using WeatherForecast.Models;
using System.Text;

namespace WeatherForecast.Authentication
{
    public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IDbService _db;
        private readonly HashService _hashService;
        private readonly IMemoryCache _cache;

        public BasicAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options
                                  , HashService hashService
                                  , ILoggerFactory logger
                                  , IMemoryCache cache
                                  , UrlEncoder encoder
                                  , ISystemClock clock
                                  , IDbService db)
            : base(options, logger, encoder, clock)
        {
            _db = db;
            _cache = cache;
            _hashService = hashService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            try
            {
                var encodedUsernamePassword = Request.Headers["Authorization"].ToString().Split(" ")[1];
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword)).Split(":");
                var username = credentials[0];
                var password = _hashService.HashPassword(credentials[1]);

                User user = null;

                if (user == null)
                    user = _db.GetUserByName(username);

                if (user.Name != username || user.Password != password)
                    return AuthenticateResult.Fail("Invalid username or password");

                var identity = new ClaimsIdentity(Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                await CreateCache(username);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail("Error while decoding username and password");
            }
        }

        private async Task CreateCache(string userName)
        {
            _cache.Set("userName", userName, TimeSpan.FromMinutes(10));
        }
    }
}
