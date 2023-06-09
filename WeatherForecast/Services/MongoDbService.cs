﻿using WeatherForecast.Models;
using MongoDB.Driver;

namespace WeatherForecast.Services
{
    public class MongoDbService : IDbService
    {
        private readonly IMongoCollection<User> _collection;

        public MongoDbService()
        {
            var urlBuilder = new MongoUrlBuilder("mongodb://db:27017/WeatherForecast");
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
            try
            {
                var result = _collection.Find(x => x.Name == name).First();

                return result;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
