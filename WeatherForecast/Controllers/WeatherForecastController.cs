using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using WeatherForecast.Models;
using WeatherForecast.Services;
using static System.Net.WebRequestMethods;

namespace WeatherForecast.Controllers
{
    //[ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;
        private readonly CEPService _CEPservice;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient, CEPService CEPService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _CEPservice = CEPService;
        }

        [HttpPost]
        [Route("/GetWeatherForecast/{cep}")]
        public string GetWeatherForecast(string cep)
        {
            if (!_CEPservice.isValidCep(cep))
                return String.Empty;

            var cepInfosRoute = $"https://viacep.com.br/ws/{cep}/json/";
            var result = _httpClient.GetAsync(cepInfosRoute).Result;
            var cepInfos = result.Content.ReadAsStringAsync().Result;
            var city = JsonConvert.DeserializeObject<LocationInfos>(cepInfos).Localidade;

            var cityId = _CEPservice.searchCityId(city);
            var weatherForecastRoute = $"http://servicos.cptec.inpe.br/XML/cidade/{cityId}/previsao.xml";
            var aresult = _httpClient.GetAsync(weatherForecastRoute).Result;
            var weatherForecast = aresult.Content.ReadAsStringAsync().Result;

            return weatherForecast;
        }


    }
}