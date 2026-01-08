using Assignment_A1_02.Models;
using Assignment_A1_02.Services;

namespace Assignment_A1_02;

class Program
{
    static async Task Main(string[] args)
    {
        OpenWeatherService service = new OpenWeatherService();

        //Register the event
        //Your Code

        Task<Forecast>[] tasks = { null, null };
        Exception exception = null;
        try
        {
            double latitude = 59.5086798659495;
            double longitude = 18.2654625932976;
            eh = (sender, message) => Console.WriteLine($"Event: {message}");
            service.WeatherForecastAvailable += eh;
            //Create the two tasks and wait for comletion
            tasks[0] = service.GetForecastAsync(latitude, longitude);
            tasks[1] = service.GetForecastAsync("New York");

            await Task.WhenAll(tasks[0], tasks[1]);
        }
        catch (Exception ex)
        {
            exception = ex;
            //How to handle an exception
            System.Console.WriteLine("ERROR!!!!!");
            //Your Code
        }

        foreach (var task in tasks)
        {
            //How to deal with successful and fault tasks
            if (task.IsCompletedSuccessfully)
            {
                var forecast = task.Result;
                System.Console.WriteLine($"\nSuccess - {forecast.City} forecast coming.\n=====================================");
                DateTime? pd = null;
                foreach (var item in forecast.Items)
                {
                    if(pd == null || item.DateTime.Date != pd.Value.Date)
                    {
                        Console.WriteLine($"\n---------- {item.DateTime:yyyy-MM-dd} ----------");
                        pd = item.DateTime;
                    }
                    Console.WriteLine($"{item.DateTime:HH:mm} | Temp: {item.Temperature}°C | Wind: {item.WindSpeed}M/s");
                }
            }
                
            
            else
                System.Console.WriteLine("Something went wrong, try again.");
        }
    }

    //Event handler declaration
    static EventHandler<string> eh;
    //Your Code
}
