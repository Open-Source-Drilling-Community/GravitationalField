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
    {
        var properties = new JsonObject
        {
            ["gravitationalField"] = new JsonObject
            {
                ["type"] = "object"
            }
        };
        var required = new JsonArray
        {
            "gravitationalField"
        };

        if (includeId)
        {
            properties["id"] = new JsonObject
            {
                ["type"] = "string",
                ["format"] = "uuid"
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
}

internal sealed class GetAllGravitationalFieldIdsMcpTool : GravitationalFieldToolBase
{
    public GetAllGravitationalFieldIdsMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field.get_all_ids";

    public override string Description => "Retrieve all Gravitational field identifiers.";

    public override JsonNode? InputSchema => null;

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

    public override string Name => "gravitational_field.get_all_meta_info";

    public override string Description => "Retrieve metadata for all Gravitational fields.";

    public override JsonNode? InputSchema => null;

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

    public override string Name => "gravitational_field.get_by_id";

    public override string Description => "Retrieve an Gravitational field by identifier.";

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

    public override string Name => "gravitational_field.get_all";

    public override string Description => "Retrieve all Gravitational fields with full data.";

    public override JsonNode? InputSchema => null;

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
                ["type"] = "boolean"
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

    public override string Name => "gravitational_field.get_all_completed";

    public override string Description => "Retrieve all Gravitational fields filtered by completion status.";

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

    public override string Name => "gravitational_field.create";

    public override string Description => "Create an Gravitational field.";

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

    public override string Name => "gravitational_field.update_by_id";

    public override string Description => "Update an existing Gravitational field identified by id.";

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

    public override string Name => "gravitational_field.delete_by_id";

    public override string Description => "Delete an Gravitational field by identifier.";

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

