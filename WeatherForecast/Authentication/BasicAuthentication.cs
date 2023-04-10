using Microsoft.AspNetCore.Authentication;
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
        private readonly DbService _db;
        private readonly HashService _hashService;

        public BasicAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options
                                  , HashService hashService
                                  , ILoggerFactory logger
                                  , UrlEncoder encoder
                                  , ISystemClock clock
                                  , DbService db)
            : base(options, logger, encoder, clock)
        {
            _db = db;
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

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail("Error while decoding username and password");
            }
        }
    }
}
