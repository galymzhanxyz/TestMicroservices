using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using WeatherMicroService.Models;
using WeatherMicroService.Services;
using WeatherMicroService.Settings;

namespace WeatherMicroService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController
    {
        private readonly OpenWeatherApiService _openWeatherApiService;
        private readonly IMongoCollection<Root> _logs;

        public WeatherController(OpenWeatherApiService openWeatherApiService, IOptions<MongoSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.DatabaseName);
            _logs = database.GetCollection<Root>(options.Value.CollectionName);
            _openWeatherApiService = openWeatherApiService;
        }

        // GET: api/[controller]/lat=43.238949&lon=76.889709
        [HttpGet("{lat}/{lon}")]
        public async Task<bool> Get(double lat = 10.99, double lon = 44.34)
        {
            var apiRespnse = _openWeatherApiService.GetResponse(lat, lon);
            var serializedLog = JsonConvert.DeserializeObject<Root>(apiRespnse);

            var loggedRecord = await (await _logs.FindAsync(x => x.id == serializedLog.id)).FirstOrDefaultAsync();

            if (loggedRecord == null)
            {
                await _logs.InsertOneAsync(serializedLog);
            }

            return true;
        }
    }
}
