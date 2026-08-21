using System;
using System.Text.Json.Nodes;

namespace NORCE.Drilling.GravitationalField.Service.Mcp.Tools;

internal static class McpToolArgumentHelpers
{
    public static JsonObject CreateEmptySchema() => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject(),
        ["additionalProperties"] = false
    };

    public static JsonObject CreateGuidSchema(string key, string? description = null)
    {
        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                [key] = new JsonObject
                {
                    ["type"] = "string",
                    ["format"] = "uuid",
                    ["description"] = description ?? "UUID of the persistent resource."
                }
            },
            ["required"] = new JsonArray
            {
                key
            },
            ["additionalProperties"] = false
        };
    }

    public static JsonObject CreateGravitationalFieldSchema(bool includeId)
    {
        JsonObject properties = new()
        {
            ["gravitationalField"] = CreateGravitationalFieldObjectSchema()
        };
        JsonArray required = new("gravitationalField");
        if (includeId)
        {
            properties["id"] = new JsonObject
            {
                ["type"] = "string",
                ["format"] = "uuid",
                ["description"] = "UUID of the persistent field to update; it must match gravitationalField.MetaInfo.ID."
            };
            required.Add("id");
        }

        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["required"] = required,
            ["additionalProperties"] = false
        };
    }

    public static JsonObject CreateCalculationOrderSchema(bool includeId)
    {
        JsonObject orderProperties = CreateResourceProperties();
        orderProperties["RawGravitationalField"] = CreateGravitationalFieldObjectSchema();
        orderProperties["CompletedGravitationalField"] = new JsonObject
        {
            ["description"] = "Calculated EGM96 output populated by the service. Omit this property when submitting a new calculation.",
            ["oneOf"] = new JsonArray(CreateGravitationalFieldObjectSchema(), new JsonObject { ["type"] = "null" })
        };

        JsonObject properties = new()
        {
            ["gravitationalFieldCalculationOrder"] = new JsonObject
            {
                ["type"] = "object",
                ["description"] = "Persistent EGM96 calculation case. Submit raw WGS84 positions and retrieve the completed field using the order UUID.",
                ["properties"] = orderProperties,
                ["required"] = new JsonArray("MetaInfo", "RawGravitationalField"),
                ["additionalProperties"] = false
            }
        };
        JsonArray required = new("gravitationalFieldCalculationOrder");
        if (includeId)
        {
            properties["id"] = new JsonObject
            {
                ["type"] = "string",
                ["format"] = "uuid",
                ["description"] = "UUID of the persistent calculation order to update; it must match gravitationalFieldCalculationOrder.MetaInfo.ID."
            };
            required.Add("id");
        }

        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties,
            ["required"] = required,
            ["additionalProperties"] = false
        };
    }

    private static JsonObject CreateGravitationalFieldObjectSchema()
    {
        JsonObject properties = CreateResourceProperties();
        properties["Type"] = new JsonObject
        {
            ["type"] = "string",
            ["enum"] = new JsonArray("Raw", "Completed"),
            ["description"] = "Raw for input positions; Completed for service-calculated gravity vectors."
        };
        properties["GravitationalDataTable"] = new JsonObject
        {
            ["type"] = "array",
            ["minItems"] = 1,
            ["description"] = "Position samples and, for completed data, their EGM96 gravity vectors.",
            ["items"] = CreateGravitationalDataSchema()
        };
        return new JsonObject
        {
            ["type"] = "object",
            ["description"] = "A named set of WGS84 positions or completed EGM96 gravity results.",
            ["properties"] = properties,
            ["required"] = new JsonArray("MetaInfo", "Type", "GravitationalDataTable"),
            ["additionalProperties"] = false
        };
    }

    private static JsonObject CreateGravitationalDataSchema() => new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["Latitude"] = new JsonObject { ["type"] = "number", ["minimum"] = -Math.PI / 2, ["maximum"] = Math.PI / 2, ["description"] = "WGS84 geodetic latitude in SI radians, from -pi/2 to pi/2." },
            ["Longitude"] = new JsonObject { ["type"] = "number", ["minimum"] = -Math.PI, ["maximum"] = Math.PI, ["description"] = "WGS84 geodetic longitude in SI radians, from -pi to pi." },
            ["Depth"] = new JsonObject { ["type"] = "number", ["description"] = "True vertical depth below the WGS84 ellipsoid in metres, positive downward. The service converts this to ellipsoidal height for GeographicLib." },
            ["GravityIntensityX"] = NullableNumber("Calculated easterly gravity-acceleration component in metres per second squared (m/s^2). Omit for raw input."),
            ["GravityIntensityY"] = NullableNumber("Calculated northerly gravity-acceleration component in metres per second squared (m/s^2). Omit for raw input."),
            ["GravityIntensityZ"] = NullableNumber("Calculated upward gravity-acceleration component in metres per second squared (m/s^2), normally negative. Omit for raw input.")
        },
        ["required"] = new JsonArray("Latitude", "Longitude", "Depth"),
        ["additionalProperties"] = false
    };

    private static JsonObject CreateResourceProperties() => new()
    {
        ["MetaInfo"] = new JsonObject
        {
            ["type"] = "object",
            ["description"] = "Resource identity metadata.",
            ["properties"] = new JsonObject { ["ID"] = new JsonObject { ["type"] = "string", ["format"] = "uuid", ["description"] = "Stable resource UUID assigned by the caller." } },
            ["required"] = new JsonArray("ID"),
            ["additionalProperties"] = true
        },
        ["Name"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["description"] = "Human-readable resource name." },
        ["Description"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["description"] = "Human-readable purpose or provenance." },
        ["CreationDate"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["format"] = "date-time" },
        ["LastModificationDate"] = new JsonObject { ["type"] = new JsonArray("string", "null"), ["format"] = "date-time" }
    };

    private static JsonObject NullableNumber(string description) => new()
    {
        ["type"] = new JsonArray("number", "null"),
        ["description"] = description
    };

    public static JsonObject CreateStringSchema(string key)
    {
        return new JsonObject
        {
            ["type"] = "object",
            ["properties"] = new JsonObject
            {
                [key] = new JsonObject
                {
                    ["type"] = "string"
                }
            },
            ["required"] = new JsonArray
            {
                key
            },
            ["additionalProperties"] = false
        };
    }

    public static bool TryParseGuid(JsonObject? arguments, string key, out Guid value, out JsonNode? error)
    {
        value = Guid.Empty;
        error = null;

        var node = arguments?[key];
        if (node is null)
        {
            error = McpToolResponses.CreateValidationError($"Argument '{key}' is required.");
            return false;
        }

        if (!Guid.TryParse(node.ToString(), out value))
        {
            error = McpToolResponses.CreateValidationError($"Argument '{key}' must be a valid UUID.");
            return false;
        }

        return true;
    }

    public static bool TryParseString(JsonObject? arguments, string key, out string value, out JsonNode? error)
    {
        value = string.Empty;
        error = null;

        var node = arguments?[key];
        if (node is null)
        {
            error = McpToolResponses.CreateValidationError($"Argument '{key}' is required.");
            return false;
        }

        value = node.ToString();
        if (string.IsNullOrWhiteSpace(value))
        {
            error = McpToolResponses.CreateValidationError($"Argument '{key}' must be a non-empty string.");
            return false;
        }

        return true;
    }

    public static bool TryParseDouble(JsonObject? arguments, string key, out double value, out JsonNode? error)
    {
        value = 0d;
        error = null;

        var node = arguments?[key];
        if (node is null)
        {
            error = McpToolResponses.CreateValidationError($"Argument '{key}' is required.");
            return false;
        }

        try
        {
            value = node.GetValue<double>();
        }
        catch (Exception ex) when (ex is InvalidOperationException or FormatException)
        {
            error = McpToolResponses.CreateValidationError($"Argument '{key}' must be a number.");
            return false;
        }

        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            error = McpToolResponses.CreateValidationError($"Argument '{key}' must be a finite number.");
            return false;
        }

        return true;
    }
}



