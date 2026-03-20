using System.Text.Json.Serialization;

namespace WeatherApp.Infrastructure.Models;

public class WeatherApiResponse
{
    [JsonPropertyName("location")]
    public LocationData? Location { get; set; }

    [JsonPropertyName("current")]
    public CurrentData? Current { get; set; }
}

public class LocationData
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

public class CurrentData
{
    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }

    [JsonPropertyName("condition")]
    public ConditionData? Condition { get; set; }
}

public class ConditionData
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
