using Microsoft.AspNetCore.Routing;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    public class CEPService
    {
        private readonly HttpClient _httpClient;

        public CEPService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public bool isValidCep(string cep)
        {
            const string PATTERN = "^[0-9]{8}$";

            if (cep != null && Regex.IsMatch(cep, PATTERN))
                return true;
            
            return false;
        }

        public int searchCityId(string cityName)
        {
            var normalizedName = NormalizeCityName(cityName);

            var route = $"http://servicos.cptec.inpe.br/XML/listaCidades?city={normalizedName}";

            var result = _httpClient.GetAsync(route).Result;

            var citiesInfos = result.Content.ReadAsStringAsync().Result;

            var deserializedCityInfos = DeserializeCityInfos(citiesInfos);

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

        private Cidades DeserializeCityInfos(string content)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Cidades));

            using (TextReader reader = new StringReader(content))
            {
                return (Cidades)serializer.Deserialize(reader);
            }
        }
    }
}
