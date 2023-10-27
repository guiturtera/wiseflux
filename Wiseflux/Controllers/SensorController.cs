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
    public class SensorController : Controller
    {
        private readonly SensorService _sensorService;

        public SensorController(SensorService sensorService)
        {
            _sensorService = sensorService;
        }

        /// <summary>
        /// Returns all user sensors
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<IEnumerable<Sensor>>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult> GetAllSensors()
        {
            var result = await _sensorService.GetAllSensors(User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Returns all user sensors
        /// </summary>
        [HttpPost("get/{sensorId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<Sensor>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult> GetSensor(int sensorId)
        {
            var result = await _sensorService.GetSensor(sensorId, User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }

        /// <summary>
        /// Returns all user sensors
        /// </summary>
        [HttpPost("add")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ServiceResponse<object>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(void))]
        [Authorize]
        public async Task<ActionResult> AddSensor([FromBody] SensorRequest sensorRequest)
        {
            var result = await _sensorService.AddSensor(sensorRequest, User);
            Response.StatusCode = (int)result.Status;

            return new JsonResult(result);
        }
    }
}
