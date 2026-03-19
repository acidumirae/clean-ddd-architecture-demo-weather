namespace WeatherApp.Domain.Entities;

public class Weather
{
    public string Location { get; set; } = string.Empty;
    public double TemperatureC { get; set; }
    public double TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Description { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
