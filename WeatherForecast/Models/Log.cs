namespace WeatherForecast.Models
{
    public class Log
    {
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public string Response { get; set; }
        public bool Success { get; set; }
    }
}
