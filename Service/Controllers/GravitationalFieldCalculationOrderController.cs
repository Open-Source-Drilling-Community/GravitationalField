using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using NORCE.Drilling.GravitationalField.Service.Managers;

namespace NORCE.Drilling.GravitationalField.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class GravitationalFieldCalculationOrderController : ControllerBase
    {
        private readonly ILogger<GravitationalFieldCalculationOrderManager> _logger;
        private readonly GravitationalFieldCalculationOrderManager _gravitationalFieldCalculationOrderManager;

        public GravitationalFieldCalculationOrderController(ILogger<GravitationalFieldCalculationOrderManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _gravitationalFieldCalculationOrderManager = GravitationalFieldCalculationOrderManager.GetInstance(_logger, connectionManager);
        }

        /// <summary>
        /// Returns the list of Guid of all GravitationalFieldCalculationOrder present in the microservice database at endpoint GravitationalField/api/GravitationalFieldCalculationOrder
        /// </summary>
        /// <returns>the list of Guid of all GravitationalFieldCalculationOrder present in the microservice database at endpoint GravitationalField/api/GravitationalFieldCalculationOrder</returns>
        [HttpGet(Name = "GetAllGravitationalFieldCalculationOrderId")]
        public ActionResult<IEnumerable<Guid>> GetAllGravitationalFieldCalculationOrderId()
        {
            var ids = _gravitationalFieldCalculationOrderManager.GetAllGravitationalFieldCalculationOrderId();
            if (ids != null)
            {
                return Ok(ids);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Returns the list of MetaInfo of all GravitationalFieldCalculationOrder present in the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/MetaInfo
        /// </summary>
        /// <returns>the list of MetaInfo of all GravitationalFieldCalculationOrder present in the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/MetaInfo</returns>
        [HttpGet("MetaInfo", Name = "GetAllGravitationalFieldCalculationOrderMetaInfo")]
        public ActionResult<IEnumerable<MetaInfo>> GetAllGravitationalFieldCalculationOrderMetaInfo()
        {
            var vals = _gravitationalFieldCalculationOrderManager.GetAllGravitationalFieldCalculationOrderMetaInfo();
            if (vals != null)
            {
                return Ok(vals);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Returns the GravitationalFieldCalculationOrder identified by its Guid from the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the GravitationalFieldCalculationOrder identified by its Guid from the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/id</returns>
        [HttpGet("{id}", Name = "GetGravitationalFieldCalculationOrderById")]
        public ActionResult<Model.GravitationalFieldCalculationOrder?> GetGravitationalFieldCalculationOrderById(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                var val = _gravitationalFieldCalculationOrderManager.GetGravitationalFieldCalculationOrderById(id);
                if (val != null)
                {
                    return Ok(val);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Returns the list of all GravitationalFieldCalculationOrderLight present in the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/LightData
        /// </summary>
        /// <returns>the list of all GravitationalFieldCalculationOrderLight present in the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/LightData</returns>
        [HttpGet("LightData", Name = "GetAllGravitationalFieldCalculationOrderLight")]
        public ActionResult<IEnumerable<Model.GravitationalFieldCalculationOrderLight>> GetAllGravitationalFieldCalculationOrderLight()
        {
            var vals = _gravitationalFieldCalculationOrderManager.GetAllGravitationalFieldCalculationOrderLight();
            if (vals != null)
            {
                return Ok(vals);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Returns the list of all GravitationalFieldCalculationOrder present in the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/HeavyData
        /// </summary>
        /// <returns>the list of all GravitationalFieldCalculationOrder present in the microservice database, at endpoint GravitationalField/api/GravitationalFieldCalculationOrder/HeavyData</returns>
        [HttpGet("HeavyData", Name = "GetAllGravitationalFieldCalculationOrder")]
        public ActionResult<IEnumerable<Model.GravitationalFieldCalculationOrder?>> GetAllGravitationalFieldCalculationOrder()
        {
            var vals = _gravitationalFieldCalculationOrderManager.GetAllGravitationalFieldCalculationOrder();
            if (vals != null)
            {
                return Ok(vals);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Performs calculation on the given GravitationalFieldCalculationOrder and adds it to the microservice database, at the endpoint GravitationalField/api/GravitationalFieldCalculationOrder
        /// </summary>
        /// <param name="gravitationalFieldCalculationOrder"></param>
        /// <returns>true if the given GravitationalFieldCalculationOrder has been added successfully to the microservice database, at the endpoint GravitationalField/api/GravitationalFieldCalculationOrder</returns>
        [HttpPost(Name = "PostGravitationalFieldCalculationOrder")]
        public ActionResult PostGravitationalFieldCalculationOrder([FromBody] Model.GravitationalFieldCalculationOrder? data)
        {
            // Check if gravitationalFieldCalculationOrder exists in the database through ID
            if (data != null && data.MetaInfo != null && data.MetaInfo.ID != Guid.Empty)
            {
                var existingData = _gravitationalFieldCalculationOrderManager.GetGravitationalFieldCalculationOrderById(data.MetaInfo.ID);
                if (existingData == null)
                {   
                    //  If gravitationalFieldCalculationOrder was not found, call AddGravitationalFieldCalculationOrder, where the gravitationalFieldCalculationOrder.Calculate()
                    // method is called. 
                    if (_gravitationalFieldCalculationOrderManager.AddGravitationalFieldCalculationOrder(data))
                    {
                        return Ok(); // status=OK is used rather than status=Created because NSwag auto-generated controllers use 200 (OK) rather than 201 (Created) as return codes
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                else
                {
                    _logger.LogWarning("The given GravitationalFieldCalculationOrder already exists and will not be added");
                    return StatusCode(StatusCodes.Status409Conflict);
                }
            }
            else
            {
                _logger.LogWarning("The given GravitationalFieldCalculationOrder is null, badly formed, or its ID is empty");
                return BadRequest();
            }
        }

        /// <summary>
        /// Performs calculation on the given GravitationalFieldCalculationOrder and updates it in the microservice database, at the endpoint GravitationalField/api/GravitationalFieldCalculationOrder/id
        /// </summary>
        /// <param name="gravitationalFieldCalculationOrder"></param>
        /// <returns>true if the given GravitationalFieldCalculationOrder has been updated successfully to the microservice database, at the endpoint GravitationalField/api/GravitationalFieldCalculationOrder/id</returns>
        [HttpPut("{id}", Name = "PutGravitationalFieldCalculationOrderById")]
        public ActionResult PutGravitationalFieldCalculationOrderById(Guid id, [FromBody] Model.GravitationalFieldCalculationOrder? data)
        {
            // Check if GravitationalFieldCalculationOrder is in the data base
            if (data != null && data.MetaInfo != null && data.MetaInfo.ID.Equals(id))
            {
                var existingData = _gravitationalFieldCalculationOrderManager.GetGravitationalFieldCalculationOrderById(id);
                if (existingData != null)
                {
                    if (_gravitationalFieldCalculationOrderManager.UpdateGravitationalFieldCalculationOrderById(id, data))
                    {
                        return Ok();
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
                else
                {
                    _logger.LogWarning("The given GravitationalFieldCalculationOrder has not been found in the database");
                    return NotFound();
                }
            }
            else
            {
                _logger.LogWarning("The given GravitationalFieldCalculationOrder is null, badly formed, or its does not match the ID to update");
                return BadRequest();
            }
        }

        /// <summary>
        /// Deletes the GravitationalFieldCalculationOrder of given ID from the microservice database, at the endpoint GravitationalField/api/GravitationalFieldCalculationOrder/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the GravitationalFieldCalculationOrder was deleted from the microservice database, at the endpoint GravitationalField/api/GravitationalFieldCalculationOrder/id</returns>
        [HttpDelete("{id}", Name = "DeleteGravitationalFieldCalculationOrderById")]
        public ActionResult DeleteGravitationalFieldCalculationOrderById(Guid id)
        {
            if (_gravitationalFieldCalculationOrderManager.GetGravitationalFieldCalculationOrderById(id) != null)
            {
                if (_gravitationalFieldCalculationOrderManager.DeleteGravitationalFieldCalculationOrderById(id))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
            {
                _logger.LogWarning("The GravitationalFieldCalculationOrder of given ID does not exist");
                return NotFound();
            }
        }
    }
}
