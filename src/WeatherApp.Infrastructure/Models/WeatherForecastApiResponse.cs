using System.Text.Json.Serialization;

namespace WeatherApp.Infrastructure.Models;

public class WeatherForecastApiResponse
{
    [JsonPropertyName("location")]
    public LocationData? Location { get; set; }

    [JsonPropertyName("forecast")]
    public ForecastData? Forecast { get; set; }
}

public class ForecastData
{
    [JsonPropertyName("forecastday")]
    public List<ForecastDayData> ForecastDay { get; set; } = [];
}

public class ForecastDayData
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("day")]
    public DayData? Day { get; set; }
}

public class DayData
{
    [JsonPropertyName("maxtemp_c")]
    public double MaxTempC { get; set; }

    [JsonPropertyName("mintemp_c")]
    public double MinTempC { get; set; }

    [JsonPropertyName("condition")]
    public ConditionData? Condition { get; set; }
}
