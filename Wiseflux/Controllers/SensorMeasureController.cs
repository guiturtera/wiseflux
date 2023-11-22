using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Wiseflux.Data;
using Wiseflux.Models;
using Wiseflux.Services;

namespace Wiseflux.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SensorMeasureController : Controller
    {
        private readonly SensorMeasureService _sensorMeasureService;

        public SensorMeasureController(SensorMeasureService sensorMeasureService)
        {
            _sensorMeasureService = sensorMeasureService;
        }

        /// <summary>
        /// Adds a measure to the sensor
        /// </summary>
        [HttpPost("add/{sensorId}/{sensorGuid}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<SensorMeasure>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ServiceResponse<object>))]
        public async Task<ActionResult> AddSensorMeasure([FromRoute] int sensorId, [FromRoute] Guid sensorGuid, [FromBody] SensorMeasure sensorMeasure)
        {
            var result = await _sensorMeasureService.AddMeasure(sensorMeasure, sensorId, sensorGuid);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Gets all user sensor
        /// </summary>
        [HttpGet("get/{sensorId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<IEnumerable<SensorMeasure>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult> GetAllSensorsMeasures([FromRoute] int sensorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _sensorMeasureService.GetSensorMeasures(sensorId, startDate, endDate, User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Gets all user sensor
        /// </summary>
        [HttpGet("get/averageByMoth/{sensorId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<IEnumerable<SensorMeasure>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<dynamic> GetAverageSensorsbByMonth([FromRoute] int sensorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            dynamic result = await _sensorMeasureService.GetAvarageSensorMeasuresByMonth(sensorId, startDate, endDate, User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Gets all user sensor
        /// </summary>
        [HttpGet("get/averageByDay/{sensorId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<IEnumerable<SensorMeasure>>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<dynamic> GetAverageSensorsbByDay([FromRoute] int sensorId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            dynamic result = await _sensorMeasureService.GetAvarageSensorMeasuresByDay(sensorId, startDate, endDate, User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }
    }
}
