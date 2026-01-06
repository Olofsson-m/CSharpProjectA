using Assignment_A1_01.Models;
using Assignment_A1_01.Services;

namespace Assignment_A1_01;

class Program
{
    static async Task Main(string[] args)
    {
        double latitude = 59.858131;
        double longitude = 17.644621;

        Forecast forecast = await new OpenWeatherService().GetForecastAsync(latitude, longitude);

        //Your Code to present each forecast item in a grouped list
        Console.WriteLine($"Weather forecast for {forecast.City}");

        foreach (var item in forecast.Items)
        {
            System.Console.WriteLine($" - {item.DateTime, 21}\n ||{item.Description, 15}|| Temp: {item.Temperature, 6}C Windspeed: {item.WindSpeed, 6}M/s \n");
        }
    }
}

