namespace WeatherMicroService.Settings
{
    public class WeatherApiSettings
    {
        public const string Position = "WeatherApiSettings";

        public string ApiKey { get; set; }
        public string ApiHostUrl { get; set; }
    }
}
