using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    public interface ILogService
    {
        void WriteLog(string message);
        List<Log> GetUserLogs();
    }
}
