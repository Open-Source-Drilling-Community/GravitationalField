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
        => McpToolArgumentHelpers.CreateCalculationOrderSchema(includeId);
}

internal sealed class GetAllGravitationalFieldCalculationOrderIdsMcpTool : GravitationalFieldCalculationOrderToolBase
{
    public GetAllGravitationalFieldCalculationOrderIdsMcpTool(ILoggerFactory loggerFactory, SqlConnectionManager connectionManager)
        : base(loggerFactory, connectionManager) { }

    public override string Name => "gravitational_field_calculation_order_get_all_ids";

    public override string Description => "List UUIDs for all persisted EGM96 gravitational-field calculation orders. Use an identifier with get_by_id to retrieve both the submitted raw positions and the completed gravity-vector results.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

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

    public override string Name => "gravitational_field_calculation_order_get_all_meta_info";

    public override string Description => "List metadata for every persisted gravitational-field calculation order without loading raw or completed data tables. Use this lightweight result to locate an order before retrieving it by UUID.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

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

    public override string Name => "gravitational_field_calculation_order_get_by_id";

    public override string Description => "Retrieve a complete gravitational-field calculation order by UUID, including its raw WGS84 inputs and completed EGM96 output. This is the result-retrieval step after create; delete the order afterward when it was only a temporary calculation case.";

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

    public override string Name => "gravitational_field_calculation_order_get_all_light";

    public override string Description => "Retrieve lightweight representations of every gravitational-field calculation order. This avoids transferring raw and completed sample tables and is appropriate for browsing calculation names, timestamps, and status.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

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

    public override string Name => "gravitational_field_calculation_order_get_all";

    public override string Description => "Retrieve every persisted gravitational-field calculation order with raw and completed data. The response can be large; prefer metadata or light tools for discovery and get_by_id for one selected calculation.";

    public override JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

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

    public override string Name => "gravitational_field_calculation_order_create";

    public override string Description => "Create and execute a persistent EGM96 gravity calculation case. Provide a caller-assigned order UUID and raw WGS84 samples with latitude/longitude in radians and depth below the WGS84 ellipsoid in metres positive downward. Retrieve results with gravitational_field_calculation_order_get_by_id, then optionally delete the temporary order.";

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

    public override string Name => "gravitational_field_calculation_order_update_by_id";

    public override string Description => "Replace and recalculate an existing EGM96 gravitational-field order. The id must match gravitationalFieldCalculationOrder.MetaInfo.ID. Raw WGS84 angles are radians and depth is metres positive downward; the completed output reports east, north, and up gravity acceleration in m/s^2.";

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

    public override string Name => "gravitational_field_calculation_order_delete_by_id";

    public override string Description => "Permanently delete a persisted gravitational-field calculation order by UUID, including its stored raw inputs and completed EGM96 result. This is the cleanup step for a temporary create/get calculation workflow.";

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

