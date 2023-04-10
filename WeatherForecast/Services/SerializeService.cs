using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    public class SerializeService
    {
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

        public Cidades DeserializeCityInfos(string content)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Cidades));

            using (TextReader reader = new StringReader(content))
            {
                return (Cidades)serializer.Deserialize(reader);
            }
        }

    }
}
