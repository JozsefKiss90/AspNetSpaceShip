using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShipColor
{
    RED,
    BLUE,
    GREEN,
    YELLOW,
}