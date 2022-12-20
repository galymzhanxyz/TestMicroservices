using Microsoft.Extensions.Options;
using System.Net;
using WeatherMicroService.Settings;

namespace WeatherMicroService.Services
{
    public class OpenWeatherApiService
    {
        private readonly string _apiKey;
        private readonly string _apiHostUrl;

        public OpenWeatherApiService(IOptions<WeatherApiSettings> options)
        {
            _apiKey = options.Value.ApiKey;
            _apiHostUrl = options.Value.ApiHostUrl;
        }


        public string GetResponse(double lon, double lat)
        {
            if (lon == 10.99 && lat == 44.34)
            {
                return GetMockJson();
            }

            WebClient webClient = new WebClient();
            webClient.QueryString.Add(nameof(lon), lon.ToString());
            webClient.QueryString.Add(nameof(lat), lat.ToString());
            webClient.QueryString.Add("appid", _apiKey);
            string result = webClient.DownloadString(_apiHostUrl);

            return result;
        }

        private string GetMockJson()
        {
            using StreamReader r = new StreamReader(Path.Join(Directory.GetCurrentDirectory(), "MockData/savedResponse.json"));
            return r.ReadToEnd();
        }
    }
}
