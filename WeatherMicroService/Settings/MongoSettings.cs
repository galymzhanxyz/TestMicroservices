namespace WeatherMicroService.Settings
{
    public class MongoSettings
    {
        public const string Position = "MongoSettings";
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
    }
}
