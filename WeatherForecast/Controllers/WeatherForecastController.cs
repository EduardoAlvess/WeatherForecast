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
        private readonly SerializeService _serializeService;
        private readonly ICepService _CepService;
        private readonly HttpClient _httpClient;
        private readonly ILogService _logger;

        public WeatherForecastController(HttpClient httpClient, ICepService CepService, ILogService logService, SerializeService serializeService)
        {
            _httpClient = httpClient;
            _CepService = CepService;
            _logger = logService;
            _serializeService = serializeService;
        }

        [HttpPost]
        [Authorize]
        [Route("/GetWeatherForecast/{cep}")]
        public string GetWeatherForecast(string cep)
        {
            if (!_CepService.IsValidCep(cep))
                return String.Empty;

            var locationInfosResult = _httpClient.GetAsync($"https://viacep.com.br/ws/{cep}/json/").Result;
            var locationInfosJsonString = locationInfosResult.Content.ReadAsStringAsync().Result;
            _logger.WriteLog(locationInfosJsonString);

            JObject locationInfosJson = JObject.Parse(locationInfosJsonString);
            var cityName = (string)locationInfosJson["localidade"];

            if (cityName == null)
                return String.Empty;

            var cityId = _CepService.SearchCityId(cityName);
            var weatherForecastResult = _httpClient.GetAsync($"http://servicos.cptec.inpe.br/XML/cidade/{cityId}/previsao.xml").Result;
            var weatherForecastXml = weatherForecastResult.Content.ReadAsStringAsync().Result;
            _logger.WriteLog(weatherForecastXml);

            string weatherForecastJson = _serializeService.ConvertXmlToJson(weatherForecastXml);

            string resultJson = _serializeService.MergeJson(locationInfosJsonString, weatherForecastJson);

            return resultJson;
        }

    }
}