using System.Collections.Concurrent;
using Newtonsoft.Json;

using Assignment_A1_03.Models;

namespace Assignment_A1_03.Services;

public class OpenWeatherService
{
    readonly HttpClient _httpClient = new HttpClient();
    readonly ConcurrentDictionary<(double, double, string), Forecast> _cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
    readonly ConcurrentDictionary<(string, string), Forecast> _cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();

    // Your API Key
    readonly string apiKey = "e7554cfd90a75bb0d5ddf09c26fe8499"; // Replace with your OpenWeatherMap API key

    //Event declaration
    public event EventHandler<string> WeatherForecastAvailable;
    protected virtual void OnWeatherForecastAvailable (string message)
    {
        WeatherForecastAvailable?.Invoke(this, message);
    }
    public async Task<Forecast> GetForecastAsync(string City)
    {
        //part of cache code here to check if forecast in Cache
        //generate an event that shows forecast was from cache
        //Your code
        
        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";


        //part of event and cache code here
        //generate an event with different message if cached data
        //Your code
        Forecast forecast = null;
        if(!_cachedCityForecasts.TryGetValue((City, apiKey), out forecast))
        {
            forecast = await ReadWebApiAsync(uri);
            _cachedCityForecasts[(City, apiKey)] = forecast;
            OnWeatherForecastAvailable($"Forecast retreived for {forecast.City}");
        }
        else
        {
            OnWeatherForecastAvailable($"[CACHED] Forecast retreived for {forecast.City}");
        }

        return forecast;

    }
    public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
    {
        //part of cache code here to check if forecast in Cache
        //generate an event that shows forecast was from cache
        //Your code

        //https://openweathermap.org/current
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

        //part of event and cache code here
        //generate an event with different message if cached data
        //Your code
        Forecast forecast = null;
        if(!_cachedGeoForecasts.TryGetValue((latitude, longitude, apiKey), out forecast))
        {
            forecast = await ReadWebApiAsync(uri);
            _cachedGeoForecasts[(latitude, longitude, apiKey)] = forecast;
            OnWeatherForecastAvailable($"Forecast retreived for {forecast.City}");
        }
        else
        {
            OnWeatherForecastAvailable($"[CACHED] Forecast retreived for {forecast.City}");
        }
        
        return forecast;
    }
    private async Task<Forecast> ReadWebApiAsync(string uri)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        //Convert Json to NewsResponse
        string content = await response.Content.ReadAsStringAsync();
        WeatherApiData wd = JsonConvert.DeserializeObject<WeatherApiData>(content);

        //Convert WeatherApiData to Forecast using Linq.
        //Your code
        var forecast = new Forecast
        {
            City = wd.city.name,
            Items = wd.list.Select(item => new ForecastItem
            {
                DateTime = UnixTimeStampToDateTime(item.dt),
                Temperature = item.main.temp,
                WindSpeed = item.wind.speed,
                Description = item.weather.FirstOrDefault().description
            }).ToList()
        };
        return forecast;
    }

    private DateTime UnixTimeStampToDateTime(double unixTimeStamp) => DateTime.UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
}

