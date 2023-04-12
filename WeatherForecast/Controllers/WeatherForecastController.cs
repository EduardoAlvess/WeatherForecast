using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Services;
using Newtonsoft.Json.Linq;

namespace WeatherForecast.Controllers
{
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly CEPService _CEPservice;
        private readonly ElasticService _elasticService;
        private readonly SerializeService _serializeService;

        public WeatherForecastController(HttpClient httpClient, CEPService CEPService, ElasticService elasticService, SerializeService serializeService)
        {
            _httpClient = httpClient;
            _CEPservice = CEPService;
            _elasticService = elasticService;
            _serializeService = serializeService;
        }

        [HttpPost]
        [Authorize]
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

            string weatherForecastJson = _serializeService.ConvertXmlToJson(weatherForecastXml);

            string resultJson = _serializeService.MergeJson(locationInfosJsonString, weatherForecastJson);

            return resultJson;
        }

    }
}