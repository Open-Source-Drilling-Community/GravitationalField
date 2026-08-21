using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NORCE.Drilling.GravitationalField.Model;

namespace NORCE.Drilling.GravitationalField.Service;

/// <summary>
/// Reads gravity results written before the GravityIntensity spelling was corrected,
/// while ensuring every new API and database payload uses the corrected contract.
/// </summary>
internal sealed class LegacyGravitationalDataJsonConverter : JsonConverter<GravitationalData>
{
    public override GravitationalData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement root = document.RootElement;

        return new GravitationalData
        {
            Latitude = ReadRequiredDouble(root, nameof(GravitationalData.Latitude)),
            Longitude = ReadRequiredDouble(root, nameof(GravitationalData.Longitude)),
            Depth = ReadRequiredDouble(root, nameof(GravitationalData.Depth)),
            GravityIntensityX = ReadNullableDouble(root, nameof(GravitationalData.GravityIntensityX), "GravitatyIntensityX"),
            GravityIntensityY = ReadNullableDouble(root, nameof(GravitationalData.GravityIntensityY), "GravitatyIntensityY"),
            GravityIntensityZ = ReadNullableDouble(root, nameof(GravitationalData.GravityIntensityZ), "GravitatyIntensityZ")
        };
    }

    public override void Write(Utf8JsonWriter writer, GravitationalData value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber(nameof(value.Latitude), value.Latitude);
        writer.WriteNumber(nameof(value.Longitude), value.Longitude);
        writer.WriteNumber(nameof(value.Depth), value.Depth);
        WriteNullableNumber(writer, nameof(value.GravityIntensityX), value.GravityIntensityX);
        WriteNullableNumber(writer, nameof(value.GravityIntensityY), value.GravityIntensityY);
        WriteNullableNumber(writer, nameof(value.GravityIntensityZ), value.GravityIntensityZ);
        writer.WriteEndObject();
    }

    private static double ReadRequiredDouble(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.Number)
        {
            throw new JsonException($"Property '{name}' must be a number.");
        }

        return value.GetDouble();
    }

    private static double? ReadNullableDouble(JsonElement root, string correctedName, string legacyName)
    {
        if (!root.TryGetProperty(correctedName, out JsonElement value) && !root.TryGetProperty(legacyName, out value))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Number => value.GetDouble(),
            _ => throw new JsonException($"Property '{correctedName}' must be a number or null.")
        };
    }

    private static void WriteNullableNumber(Utf8JsonWriter writer, string name, double? value)
    {
        if (value.HasValue)
        {
            writer.WriteNumber(name, value.Value);
        }
        else
        {
            writer.WriteNull(name);
        }
    }
}
