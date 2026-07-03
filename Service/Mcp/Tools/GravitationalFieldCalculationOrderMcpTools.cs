using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.GravitationalField.Service.Controllers;
using NORCE.Drilling.GravitationalField.Service.Managers;
using GravitationalFieldCalculationOrderModel = NORCE.Drilling.GravitationalField.Model.GravitationalFieldCalculationOrder;

namespace NORCE.Drilling.GravitationalField.Service.Mcp.Tools;

internal abstract class GravitationalFieldCalculationOrderToolBase : IMcpTool
{
    private protected readonly ILoggerFactory LoggerFactory;
    private protected readonly SqlConnectionManager ConnectionManager;

    protected GravitationalFieldCalculationOrderToolBase(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
    {
        LoggerFactory = loggerFactory;
        ConnectionManager = connectionManager;
    }

    public abstract string Name { get; }

    public abstract string Description { get; }

    public abstract JsonNode? InputSchema { get; }

    public abstract Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken);

    protected GravitationalFieldCalculationOrderController CreateController()
    {
        return new GravitationalFieldCalculationOrderController(
            LoggerFactory.CreateLogger<GravitationalFieldCalculationOrderManager>(),
            ConnectionManager);
    }

    protected static bool TryDeserialize(JsonObject? arguments, out GravitationalFieldCalculationOrderModel calculationOrder, out JsonNode? error)
    {
        calculationOrder = default!;
        error = null;

        if (arguments?["gravitationalFieldCalculationOrder"] is not JsonNode orderNode)
        {
            error = McpToolResponses.CreateValidationError("Argument 'gravitationalFieldCalculationOrder' is required.");
            return false;
        }

        try
        {
            calculationOrder = orderNode.Deserialize<GravitationalFieldCalculationOrderModel>(JsonSettings.Options) ?? throw new InvalidOperationException();
            return true;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            error = McpToolResponses.CreateValidationError("Argument 'gravitationalFieldCalculationOrder' could not be deserialized.");
            return false;
        }
    }

    protected static JsonObject CreateCalculationOrderSchema(bool includeId)
    {
        var properties = new JsonObject
        {
            ["gravitationalFieldCalculationOrder"] = new JsonObject
            {
                ["type"] = "object"
            }
        };
        var required = new JsonArray
        {
            "gravitationalFieldCalculationOrder"
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

internal sealed class GetAllGravitationalFieldCalculationOrderIdsMcpTool : GravitationalFieldCalculationOrderToolBase
{
    public GetAllGravitationalFieldCalculationOrderIdsMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.get_all_ids";

    public override string Description => "Retrieve all Gravitational field calculation order identifiers.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllGravitationalFieldCalculationOrderId());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllGravitationalFieldCalculationOrderMetaInfoMcpTool : GravitationalFieldCalculationOrderToolBase
{
    public GetAllGravitationalFieldCalculationOrderMetaInfoMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.get_all_meta_info";

    public override string Description => "Retrieve metadata for all Gravitational field calculation orders.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllGravitationalFieldCalculationOrderMetaInfo());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetGravitationalFieldCalculationOrderByIdMcpTool : GravitationalFieldCalculationOrderToolBase
{
    public GetGravitationalFieldCalculationOrderByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.get_by_id";

    public override string Description => "Retrieve an Gravitational field calculation order by identifier.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().GetGravitationalFieldCalculationOrderById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllGravitationalFieldCalculationOrderLightMcpTool : GravitationalFieldCalculationOrderToolBase
{
    public GetAllGravitationalFieldCalculationOrderLightMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.get_all_light";

    public override string Description => "Retrieve all Gravitational field calculation orders as lightweight records.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllGravitationalFieldCalculationOrderLight());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class GetAllGravitationalFieldCalculationOrderMcpTool : GravitationalFieldCalculationOrderToolBase
{
    public GetAllGravitationalFieldCalculationOrderMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.get_all";

    public override string Description => "Retrieve all Gravitational field calculation orders with full data.";

    public override JsonNode? InputSchema => null;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var response = McpActionResultConverter.FromActionResult(CreateController().GetAllGravitationalFieldCalculationOrder());
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class PostGravitationalFieldCalculationOrderMcpTool : GravitationalFieldCalculationOrderToolBase
{
    private static readonly JsonObject Schema = CreateCalculationOrderSchema(includeId: false);

    public PostGravitationalFieldCalculationOrderMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.create";

    public override string Description => "Calculate and create an Gravitational field calculation order.";

    public override JsonNode? InputSchema => Schema;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!TryDeserialize(arguments, out GravitationalFieldCalculationOrderModel calculationOrder, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().PostGravitationalFieldCalculationOrder(calculationOrder));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class PutGravitationalFieldCalculationOrderByIdMcpTool : GravitationalFieldCalculationOrderToolBase
{
    private static readonly JsonObject Schema = CreateCalculationOrderSchema(includeId: true);

    public PutGravitationalFieldCalculationOrderByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.update_by_id";

    public override string Description => "Calculate and update an existing Gravitational field calculation order identified by id.";

    public override JsonNode? InputSchema => Schema;

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? idError))
        {
            return Task.FromResult<JsonNode?>(idError);
        }
        if (!TryDeserialize(arguments, out GravitationalFieldCalculationOrderModel calculationOrder, out JsonNode? orderError))
        {
            return Task.FromResult<JsonNode?>(orderError);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().PutGravitationalFieldCalculationOrderById(id, calculationOrder));
        return Task.FromResult<JsonNode?>(response);
    }
}

internal sealed class DeleteGravitationalFieldCalculationOrderByIdMcpTool : GravitationalFieldCalculationOrderToolBase
{
    public DeleteGravitationalFieldCalculationOrderByIdMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order.delete_by_id";

    public override string Description => "Delete an Gravitational field calculation order by identifier.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateGuidSchema("id");

    public override Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!McpToolArgumentHelpers.TryParseGuid(arguments, "id", out Guid id, out JsonNode? error))
        {
            return Task.FromResult<JsonNode?>(error);
        }

        var response = McpActionResultConverter.FromActionResult(CreateController().DeleteGravitationalFieldCalculationOrderById(id));
        return Task.FromResult<JsonNode?>(response);
    }
}

