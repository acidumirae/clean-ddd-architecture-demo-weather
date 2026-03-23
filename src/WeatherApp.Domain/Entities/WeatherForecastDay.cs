namespace WeatherApp.Domain.Entities;

public class WeatherForecastDay
{
    public DateOnly Date { get; set; }
    public double MaxTempC { get; set; }
    public double MinTempC { get; set; }
    public string Description { get; set; } = string.Empty;
}
