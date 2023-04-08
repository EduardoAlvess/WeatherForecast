using Elastic.Apm.Api;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using WeatherForecast.Models;

namespace WeatherForecast.Services
{
    public class ElasticService
    {
        private readonly ElasticClient _client;

        public ElasticService()
        {
            var settings = new ConnectionSettings().DefaultIndex("logs");
            _client = new ElasticClient(settings);
        }

        public void WriteLog(string message)
        {
            var log = new Log
            {
                UserId = 1,
                CreationDate = DateTime.Now,
                Response = message
            };

            _client.IndexDocument(log);
        }

        public List<Log> GetUserLogs()
        {
            //_client.DeleteByQuery<Log>(q => q
            //        .Query(rq => rq
            //        .Term(f => f.UserId, "1")
            //        )
            //        );

            //return new List<Log>();

            var searchResponse = _client.Search<Log>(s => s
                .From(0)
                .Size(10)
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.UserId)
                        .Query("1")
                    )
                )
            );

            var logs = searchResponse.Documents.ToList();

            return logs;
        }
    }
}
