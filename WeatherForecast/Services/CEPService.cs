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

        public CEPService(HttpClient httpClient, ElasticService elasticService)
        {
            _httpClient = httpClient;
            _elasticService = elasticService;
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

            var deserializedCityInfos = DeserializeCityInfos(citiesInfos);

            return deserializedCityInfos.City.ID;
        }

        public string ConvertXmlToJson(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return JsonConvert.SerializeXmlNode(doc);
        }

        public string MergeJson(string json1, string json2)
        {
            JObject obj1 = JObject.Parse(json1);
            JObject obj2 = JObject.Parse(json2);

            obj1.Merge(obj2);

            return obj1.ToString();
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
