namespace WeatherForecast.Exceptions
{
    public class UserAlreadyExistException : Exception
    {
        public override string Message { get; }
        public override string StackTrace => "";

        public UserAlreadyExistException()
        {
            Message = "User already exists.";
        }
    }
}
