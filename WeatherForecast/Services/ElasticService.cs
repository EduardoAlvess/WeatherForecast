﻿using Microsoft.Extensions.Caching.Memory;
using WeatherForecast.Models;
using Nest;

namespace WeatherForecast.Services
{
    public class ElasticService : ILogService
    {
        private readonly ElasticClient _client;
        private readonly IMemoryCache _cache;
        private readonly IDbService _db;

        public ElasticService(IDbService db, IMemoryCache cache)
        {
            var connectionString = new Uri("http://elastic:9200");
            var settings = new ConnectionSettings(connectionString).DefaultIndex("logs");
            _client = new ElasticClient(settings);
            _cache = cache;
            _db = db;
        }

        public void WriteLog(string message)
        {
            var name = _cache.Get("userName").ToString();

            var userId = _db.GetUserByName(name).Id.ToString();

            var log = new Log
            {
                UserId = userId,
                CreationDate = DateTime.Now,
                Response = message
            };

            _client.IndexDocument(log);
        }

        public List<Log> GetUserLogs()
        {
            var name = _cache.Get("userName").ToString();

            var userId = _db.GetUserByName(name).Id.ToString();

            var searchResponse = _client.Search<Log>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.UserId)
                        .Query(userId)
                    )
                )
            );

            var logs = searchResponse.Documents.ToList();

            return logs;
        }
    }
}
