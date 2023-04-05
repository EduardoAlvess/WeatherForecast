using System.Xml.Serialization;

namespace WeatherForecast.Models
{
    [XmlRoot(ElementName = "cidades", Namespace = "")]
    public class Cidades
    {
        [XmlElement(ElementName = "cidade")]
        public Cidade City { get; set; }
    }
    public class Cidade
    {
        [XmlElement(ElementName = "nome")]
        public string Name { get; set; }

        [XmlElement(ElementName = "uf")]
        public string UF { get; set; }

        [XmlElement(ElementName = "id")]
        public int ID { get; set; }
    }

}
