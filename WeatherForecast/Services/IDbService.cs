using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    public interface IDbService
    {
        void CreateUser(User user);
        User GetUserByName(string name);
    }
}
