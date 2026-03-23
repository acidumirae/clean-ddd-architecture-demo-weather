namespace WeatherApp.Domain.Entities;

public class WeatherForecast
{
    public string Location { get; set; } = string.Empty;
    public List<WeatherForecastDay> Days { get; set; } = [];
}
