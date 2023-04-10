using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    public class CEPService
    {
        private readonly HttpClient _httpClient;
        private readonly ElasticService _elasticService;
        private readonly SerializeService _serializeService;

        public CEPService(HttpClient httpClient, ElasticService elasticService, SerializeService serializeService)
        {
            _httpClient = httpClient;
            _elasticService = elasticService;
            _serializeService = serializeService;
        }

        public bool IsValidCep(string cep)
        {
            const string PATTERN = "^[0-9]{8}$";

            if (cep != null && Regex.IsMatch(cep, PATTERN))
                return true;
            
            return false;
        }

        public int SearchCityId(string cityName)
        {
            var normalizedName = NormalizeCityName(cityName);

            var route = $"http://servicos.cptec.inpe.br/XML/listaCidades?city={normalizedName}";

            var result = _httpClient.GetAsync(route).Result;

            var citiesInfos = result.Content.ReadAsStringAsync().Result;
            _elasticService.WriteLog(citiesInfos);

            var deserializedCityInfos = _serializeService.DeserializeCityInfos(citiesInfos);

            return deserializedCityInfos.City.ID;
        }

        private string NormalizeCityName(string cityName)
        {
            StringBuilder normalizedName = new StringBuilder();

            var arrayText = cityName.Normalize(NormalizationForm.FormD).ToCharArray();

            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    normalizedName.Append(letter);
            }

            return normalizedName.ToString().ToLower();
        }
    }
}
