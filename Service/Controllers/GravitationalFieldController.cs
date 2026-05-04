using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OSDC.DotnetLibraries.General.DataManagement;
using NORCE.Drilling.GravitationalField.Model;
using NORCE.Drilling.GravitationalField.Service.Managers;

namespace NORCE.Drilling.GravitationalField.Service.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class GravitationalFieldController : ControllerBase
    {
        private readonly ILogger<GravitationalFieldManager> _logger;
        private readonly GravitationalFieldManager _gravitationalFieldManager;

        public GravitationalFieldController(ILogger<GravitationalFieldManager> logger, SqlConnectionManager connectionManager)
        {
            _logger = logger;
            _gravitationalFieldManager = GravitationalFieldManager.GetInstance(_logger, connectionManager);
        }

        /// <summary>
        /// Returns the list of Guid of all GravitationalField present in the microservice database at endpoint GravitationalField/api/GravitationalField
        /// </summary>
        /// <returns>the list of Guid of all GravitationalField present in the microservice database at endpoint GravitationalField/api/GravitationalField</returns>
        [HttpGet(Name = "GetAllGravitationalFieldId")]
        public ActionResult<IEnumerable<Guid?>> GetAllGravitationalFieldId()
        {
            var ids = _gravitationalFieldManager.GetAllGravitationalFieldId();
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
        /// Returns the list of MetaInfo of all GravitationalField present in the microservice database, at endpoint GravitationalField/api/GravitationalField/MetaInfo
        /// </summary>
        /// <returns>the list of MetaInfo of all GravitationalField present in the microservice database, at endpoint GravitationalField/api/GravitationalField/MetaInfo</returns>
        [HttpGet("MetaInfo", Name = "GetAllGravitationalFieldMetaInfo")]
        public ActionResult<IEnumerable<MetaInfo?>> GetAllGravitationalFieldMetaInfo()
        {
            var vals = _gravitationalFieldManager.GetAllGravitationalFieldMetaInfo();
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
        /// Returns the GravitationalField identified by its Guid from the microservice database, at endpoint GravitationalField/api/GravitationalField/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>the GravitationalField identified by its Guid from the microservice database, at endpoint GravitationalField/api/GravitationalField/id</returns>
        [HttpGet("{id}", Name = "GetGravitationalFieldById")]
        public ActionResult<Model.GravitationalField?> GetGravitationalFieldById(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                var val = _gravitationalFieldManager.GetGravitationalFieldById(id);
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
        /// Returns the list of all GravitationalField present in the microservice database, at endpoint GravitationalField/api/GravitationalField/HeavyData
        /// </summary>
        /// <returns>the list of all GravitationalField present in the microservice database, at endpoint GravitationalField/api/GravitationalField/HeavyData</returns>
        [HttpGet("HeavyData", Name = "GetAllGravitationalField")]
        public ActionResult<IEnumerable<Model.GravitationalField?>> GetAllGravitationalField()
        {
            var vals = _gravitationalFieldManager.GetAllGravitationalField();
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
        /// Returns the list of all GravitationalField present in the microservice database, at endpoint GravitationalField/api/GravitationalField/HeavyData
        /// </summary>
        /// <returns>the list of all GravitationalField present in the microservice database, at endpoint GravitationalField/api/GravitationalField/HeavyData</returns>
        [HttpGet("Completed", Name = "GetAllCompletedGravitationalField")]
        public ActionResult<IEnumerable<Model.GravitationalField?>> GetAllCompletedGravitationalField(bool completedBool)
        {
            var vals = _gravitationalFieldManager.GetAllCompletedGravitationalField(completedBool);
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
        /// Performs calculation on the given GravitationalField and adds it to the microservice database, at the endpoint GravitationalField/api/GravitationalField
        /// </summary>
        /// <param name="gravitationalField"></param>
        /// <returns>true if the given GravitationalField has been added successfully to the microservice database, at the endpoint GravitationalField/api/GravitationalField</returns>
        [HttpPost(Name = "PostGravitationalField")]
        public ActionResult PostGravitationalField([FromBody] Model.GravitationalField? data)
        {
            if (data != null && data.MetaInfo != null && data.MetaInfo.ID != Guid.Empty)
            {
                var existingData = _gravitationalFieldManager.GetGravitationalFieldById(data.MetaInfo.ID);
                if (existingData == null)
                {
                    if (_gravitationalFieldManager.AddGravitationalField(data))
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
                    _logger.LogWarning("The given GravitationalField already exists and will not be added");
                    return StatusCode(StatusCodes.Status409Conflict);
                }
            }
            else
            {
                _logger.LogWarning("The given GravitationalField is null or its ID is empty");
                return BadRequest();
            }
        }

        /// <summary>
        /// Performs calculation on the given GravitationalField and updates it in the microservice database, at the endpoint GravitationalField/api/GravitationalField/id
        /// </summary>
        /// <param name="gravitationalField"></param>
        /// <returns>true if the given GravitationalField has been updated successfully to the microservice database, at the endpoint GravitationalField/api/GravitationalField/id</returns>
        [HttpPut("{id}", Name = "PutGravitationalFieldById")]
        public ActionResult PutGravitationalFieldById(Guid id, [FromBody] Model.GravitationalField data)
        {
            if (data != null && data.MetaInfo != null && data.MetaInfo.ID.Equals(id))
            {
                var existingData = _gravitationalFieldManager.GetGravitationalFieldById(id);
                if (existingData != null)
                {
                    if (_gravitationalFieldManager.UpdateGravitationalFieldById(id, data))
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
                    _logger.LogWarning("The given GravitationalField has not been found in the database");
                    return NotFound();
                }
            }
            else
            {
                _logger.LogWarning("The given GravitationalField is null or its does not match the ID to update");
                return BadRequest();
            }
        }

        /// <summary>
        /// Deletes the GravitationalField of given ID from the microservice database, at the endpoint GravitationalField/api/GravitationalField/id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>true if the GravitationalField was deleted from the microservice database, at the endpoint GravitationalField/api/GravitationalField/id</returns>
        [HttpDelete("{id}", Name = "DeleteGravitationalFieldById")]
        public ActionResult DeleteGravitationalFieldById(Guid id)
        {
            if (_gravitationalFieldManager.GetGravitationalFieldById(id) != null)
            {
                if (_gravitationalFieldManager.DeleteGravitationalFieldById(id))
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
                _logger.LogWarning("The GravitationalField of given ID does not exist");
                return NotFound();
            }
        }
    }
}
