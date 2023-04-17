namespace WeatherForecast.Services
{
    public interface ICepService
    {
        bool IsValidCep(string cep);
        int SearchCityId(string cityName);
    }
}
