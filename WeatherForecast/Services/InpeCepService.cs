using System.Text.RegularExpressions;
using System.Globalization;
using System.Text;

namespace WeatherForecast.Services
{
    public class InpeCepService : ICepService
    {
        private readonly SerializeService _serializeService;
        private readonly HttpClient _httpClient;
        private readonly ILogService _logger;

        public InpeCepService(HttpClient httpClient, ILogService logService, SerializeService serializeService)
        {
            _serializeService = serializeService;
            _httpClient = httpClient;
            _logger = logService;
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
            try
            {
                var normalizedName = NormalizeCityName(cityName);

                var route = $"http://servicos.cptec.inpe.br/XML/listaCidades?city={normalizedName}";

                var result = _httpClient.GetAsync(route).Result;

                var citiesInfos = result.Content.ReadAsStringAsync().Result;
                _logger.WriteLog(citiesInfos);

                var deserializedCityInfos = _serializeService.DeserializeCityInfos(citiesInfos);

                return deserializedCityInfos.City.ID;
            }
            catch(Exception e)
            {
                throw e;
            }
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
