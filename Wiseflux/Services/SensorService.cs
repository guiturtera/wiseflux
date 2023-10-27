using System.Net;
using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Helpers;
using Wiseflux.Models;
using Wiseflux.Security;

namespace Wiseflux.Services
{
    public class SensorService
    {
        private readonly ApplicationDbContext _db;

        public SensorService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<IEnumerable<Sensor>>> GetAllSensors(ClaimsPrincipal user)
        {
            var sensors = _db.Sensors.Where(sensor => sensor.User == ClaimsHelper.GetUserEmail(user)).ToList();

            return new ServiceResponse<IEnumerable<Sensor>>(HttpStatusCode.OK, "Sucesso", sensors);
        }

        public async Task<ServiceResponse<Sensor>> GetSensor(int sensorId, ClaimsPrincipal user)
        {
            var sensor = _db.Sensors.Find(ClaimsHelper.GetUserEmail(user), sensorId);

            if (sensor == null) { return new ServiceResponse<Sensor>(HttpStatusCode.NotFound, "User not found.", null); }

            return new ServiceResponse<Sensor>(HttpStatusCode.OK, "Sucesso", sensor);
        }

        public async Task<ServiceResponse<object>> AddSensor(SensorRequest sensorRequest, ClaimsPrincipal newUser)
        {
            string userEmail = ClaimsHelper.GetUserEmail(newUser);
            Guid randomGuid = Guid.NewGuid();

            var newSensor = new Sensor()
            {
                User = userEmail,
                SensorName = sensorRequest.SensorName,
                SensorGuid = randomGuid,
                SensorType = sensorRequest.SensorType
            };

            _db.Sensors.Add(newSensor);
            _db.SaveChanges();

            return new ServiceResponse<object>(HttpStatusCode.OK, "Success", null);
        }
    }
}
