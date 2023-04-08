using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WeatherForecast.Models;
using WeatherForecast.Services;

namespace WeatherForecast.Controllers
{
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HttpClient _httpClient;
        private readonly CEPService _CEPservice;
        private readonly ElasticService _elasticService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, HttpClient httpClient, CEPService CEPService, ElasticService elasticService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _CEPservice = CEPService;
            _elasticService = elasticService;
        }

        [HttpPost]
        [Route("/GetWeatherForecast/{cep}")]
        public string GetWeatherForecast(string cep)
        {
            if (!_CEPservice.IsValidCep(cep))
                return String.Empty;

            var locationInfosResult = _httpClient.GetAsync($"https://viacep.com.br/ws/{cep}/json/").Result;
            var locationInfosJsonString = locationInfosResult.Content.ReadAsStringAsync().Result;
            _elasticService.WriteLog(locationInfosJsonString);

            JObject locationInfosJson = JObject.Parse(locationInfosJsonString);
            var cityName = (string)locationInfosJson["localidade"];

            if (cityName == null)
                return String.Empty;

            var cityId = _CEPservice.SearchCityId(cityName);
            var weatherForecastResult = _httpClient.GetAsync($"http://servicos.cptec.inpe.br/XML/cidade/{cityId}/previsao.xml").Result;
            var weatherForecastXml = weatherForecastResult.Content.ReadAsStringAsync().Result;
            _elasticService.WriteLog(weatherForecastXml);

            string weatherForecastJson = _CEPservice.ConvertXmlToJson(weatherForecastXml);

            string resultJson = _CEPservice.MergeJson(locationInfosJsonString, weatherForecastJson);

            return resultJson;
        }

        [HttpGet]
        [Route("/GetUserLogs")]
        public List<Log> GetUserLogs()
        {
            return _elasticService.GetUserLogs();
        }
    }
}