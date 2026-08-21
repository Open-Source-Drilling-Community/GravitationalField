using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.GravitationalField.Service.Controllers;
using NORCE.Drilling.GravitationalField.Service.Managers;
using GravitationalFieldModel = NORCE.Drilling.GravitationalField.Model.GravitationalField;

namespace NORCE.Drilling.GravitationalField.Service.Mcp.Tools;

internal abstract class GravitationalFieldToolBase : IMcpTool
{
    private protected readonly ILoggerFactory LoggerFactory;
    private protected readonly SqlConnectionManager ConnectionManager;

    protected GravitationalFieldToolBase(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
    {
        LoggerFactory = loggerFactory;
        ConnectionManager = connectionManager;
    }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract JsonNode? InputSchema { get; }

    public abstract Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken);

    protected GravitationalFieldController CreateController()
    {
        return new GravitationalFieldController(
            LoggerFactory.CreateLogger<GravitationalFieldManager>(),
            ConnectionManager);
    }

    protected static bool TryDeserialize(JsonObject? arguments, out GravitationalFieldModel gravitationalField, out JsonNode? error)
    {
        gravitationalField = default!;
        error = null;

        if (arguments?["gravitationalField"] is not JsonNode gravitationalFieldNode)
        {
            error = McpToolResponses.CreateValidationError("Argument 'gravitationalField' is required.");
            return false;
        }

        try
        {
            gravitationalField = gravitationalFieldNode.Deserialize<GravitationalFieldModel>(JsonSettings.Options) ?? throw new InvalidOperationException();
            return true;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            error = McpToolResponses.CreateValidationError("Argument 'gravitationalField' could not be deserialized.");
            return false;
        }
    }

    protected static JsonObject CreateGravitationalFieldSchema(bool includeId)
        => McpToolArgumentHelpers.CreateGravitationalFieldSchema(includeId);
}

internal sealed class GetAllGravitationalFieldIdsMcpTool : GravitationalFieldToolBase
{
    public GetAllGravitationalFieldIdsMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_get_all_ids";

    public override string Description => "List the UUIDs of every persisted gravitational field. Use this lightweight discovery tool before gravitational_field_get_by_id when only resource identifiers are needed; it does not return metadata or sample data.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllGravitationalFieldId());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllGravitationalFieldMetaInfoMcpTool : GravitationalFieldToolBase
{
    public GetAllGravitationalFieldMetaInfoMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_get_all_meta_info";

    public override string Description => "List identity metadata for every persisted gravitational field without loading the potentially large data tables. Use it to select a field by name or UUID before requesting its complete WGS84 positions and gravity results.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllGravitationalFieldMetaInfo());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetGravitationalFieldByIdMcpTool : GravitationalFieldToolBase
{
    public GetGravitationalFieldByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_get_by_id";

    public override string Description => "Retrieve one complete persisted gravitational field by UUID, including metadata and its data table. Positions are WGS84 latitude/longitude in radians and depth in metres positive downward; completed vectors are east, north, and up in m/s^2.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().GetGravitationalFieldById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllGravitationalFieldMcpTool : GravitationalFieldToolBase
{
    public GetAllGravitationalFieldMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_get_all";

    public override string Description => "Retrieve every persisted gravitational field with full metadata and data tables. This can return a large payload; prefer the IDs or metadata tools for discovery and get_by_id for a selected field.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllGravitationalField());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllCompletedGravitationalFieldMcpTool : GravitationalFieldToolBase
{
    private static readonly JsonObject Schema = new()
    {
        ["type"] = "object",
        ["properties"] = new JsonObject
        {
            ["isCompleted"] = new JsonObject
            {
                ["type"] = "boolean",
                ["description"] = "True selects Completed fields containing calculated vectors; false selects Raw input fields."
            }
        },
        ["required"] = new JsonArray
        {
            "isCompleted"
        },
        ["additionalProperties"] = false
    };

    public GetAllCompletedGravitationalFieldMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_get_all_completed";

    public override string Description => "Retrieve complete gravitational fields filtered by state. Set isCompleted=true for service-calculated EGM96 vectors or false for raw WGS84 position inputs. Returned angles are radians, depth is metres positive downward, and vectors are m/s^2.";

    public override JsonNode? InputSchema => Schema;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (arguments?["isCompleted"] is not JsonNode isCompletedNode)
        {
            return Task.FromResult<JsonNode?>(McpToolResponses.CreateValidationError("Argument 'isCompleted' is required."));
        }

        bool isCompleted;
        try
        {
            isCompleted = isCompletedNode.GetValue<bool>();
        }
        catch (Exception ex) when (ex is InvalidOperationException or FormatException)
        {
            return Task.FromResult<JsonNode?>(McpToolResponses.CreateValidationError("Argument 'isCompleted' must be a boolean."));
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllCompletedGravitationalField(isCompleted));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class PostGravitationalFieldMcpTool : GravitationalFieldToolBase
{
    private static readonly JsonObject Schema = CreateGravitationalFieldSchema(includeId: false);

    public PostGravitationalFieldMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_create";

    public override string Description => "Persist a gravitational-field dataset without performing a calculation. Supply a caller-assigned MetaInfo.ID and one or more WGS84 positions: latitude/longitude in SI radians and depth below the WGS84 ellipsoid in metres positive downward. Use a calculation-order tool to compute EGM96 values.";

    public override JsonNode? InputSchema => Schema;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!TryDeserialize(arguments, out GravitationalFieldModel gravitationalField, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().PostGravitationalField(gravitationalField));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class PutGravitationalFieldByIdMcpTool : GravitationalFieldToolBase
{
    private static readonly JsonObject Schema = CreateGravitationalFieldSchema(includeId: true);

    public PutGravitationalFieldByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_update_by_id";

    public override string Description => "Replace an existing persisted gravitational field. The route id must identify the resource and match gravitationalField.MetaInfo.ID. This tool stores the supplied field but does not calculate gravity; preserve the SI and WGS84 conventions described by the schema.";

    public override JsonNode? InputSchema => Schema;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? idError))
        {
            return Task.FromResult<JsonNode?>(idError);
        }
        if (!TryDeserialize(arguments, out GravitationalFieldModel gravitationalField, out JsonNode? fieldError))
        {
            return Task.FromResult<JsonNode?>(fieldError);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().PutGravitationalFieldById(id, gravitationalField));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class DeleteGravitationalFieldByIdMcpTool : GravitationalFieldToolBase
{
    public DeleteGravitationalFieldByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_delete_by_id";

    public override string Description => "Permanently delete one persisted gravitational field by UUID. Use this for obsolete standalone raw or completed datasets; deleting a field is distinct from deleting a calculation order and cannot be undone through MCP.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().DeleteGravitationalFieldById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}

