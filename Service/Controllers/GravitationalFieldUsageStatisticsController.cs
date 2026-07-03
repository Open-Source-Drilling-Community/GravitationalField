using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NORCE.Drilling.GravitationalField.Model;

namespace NORCE.Drilling.GravitationalField.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class GravitationalFieldUsageStatisticsController : ControllerBase
    {
        private readonly ILogger<GravitationalFieldUsageStatisticsController> _logger;

        public GravitationalFieldUsageStatisticsController(ILogger<GravitationalFieldUsageStatisticsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns the usage statistics present in the microservice database at endpoint GravitationalField/api/GravitationalFieldUsageStatistics
        /// </summary>
        /// <returns>the usage statistics present in the microservice database at endpoint GravitationalField/api/GravitationalFieldUsageStatistics</returns>
        [HttpGet(Name = "GetGravitationalFieldUsageStatistics")]
        public ActionResult<UsageStatisticsGravitationalField> GetGravitationalFieldUsageStatistics()
        {
            UsageStatisticsGravitationalField.Instance.IncrementGetGravitationalFieldUsageStatisticsPerDay();
            if (UsageStatisticsGravitationalField.Instance != null)
            {
                return Ok(UsageStatisticsGravitationalField.Instance);
            }

            _logger.LogWarning("Usage statistics are not available");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
