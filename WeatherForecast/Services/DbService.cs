using WeatherForecast.Models;
using MongoDB.Driver;

namespace WeatherForecast.Services
{
    public class DbService
    {
        private readonly IMongoCollection<User> _collection;

        public DbService()
        {
            var urlBuilder = new MongoUrlBuilder("mongodb://localhost:27017/WeatherForecast");
            urlBuilder.RetryWrites = false;

            var mongoUrl = urlBuilder.ToMongoUrl();

            var mongoClient = new MongoClient(mongoUrl);

            var db = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _collection = db.GetCollection<User>("Users");
        }

        public void CreateUser(User user)
        {
            _collection.InsertOneAsync(user).Wait();
        }

        public User GetUserByName(string name)
        {
            var result = _collection.Find(x => x.Name == name).First();

            return result;
        }
    }
}
