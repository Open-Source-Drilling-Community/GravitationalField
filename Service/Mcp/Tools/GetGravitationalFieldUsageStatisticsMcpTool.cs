using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.GravitationalField.Service.Controllers;

namespace NORCE.Drilling.GravitationalField.Service.Mcp.Tools;

internal sealed class GetGravitationalFieldUsageStatisticsMcpTool : IMcpTool
{
    private readonly ILoggerFactory _loggerFactory;

    public GetGravitationalFieldUsageStatisticsMcpTool(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public string Name => "gravitational_field_usage_statistics_get";

    public string Description => "Retrieve aggregate invocation counters for the GravitationalField HTTP endpoints. This operational tool reports service usage only; it does not return gravitational-field samples, calculation results, or physical model data.";

    public JsonNode? InputSchema => McpToolArgumentHelpers.CreateEmptySchema();

    public Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var controller = new GravitationalFieldUsageStatisticsController(
            _loggerFactory.CreateLogger<GravitationalFieldUsageStatisticsController>());
        var response = McpActionResultConverter.FromActionResult(controller.GetGravitationalFieldUsageStatistics());
        return Task.FromResult<JsonNode?>(response);
    }
}

