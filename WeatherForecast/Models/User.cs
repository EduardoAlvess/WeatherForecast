using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherForecast.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
