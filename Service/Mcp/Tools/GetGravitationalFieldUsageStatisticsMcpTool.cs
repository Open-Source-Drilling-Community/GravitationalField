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

    public string Name => "gravitational_field_usage_statistics.get";

    public string Description => "Retrieve usage statistics for the GravitationalField microservice.";

    public JsonNode? InputSchema => null;

    public Task<JsonNode?> InvokeAsync(JsonObject? arguments, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var controller = new GravitationalFieldUsageStatisticsController(
            _loggerFactory.CreateLogger<GravitationalFieldUsageStatisticsController>());
        var response = McpActionResultConverter.FromActionResult(controller.GetGravitationalFieldUsageStatistics());
        return Task.FromResult<JsonNode?>(response);
    }
}

