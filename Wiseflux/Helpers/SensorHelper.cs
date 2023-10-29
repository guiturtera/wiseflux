using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Models;

namespace Wiseflux.Helpers
{
    public static class SensorHelper
    {
        public static Sensor GetSensorFromUser(ApplicationDbContext db, int sensorId, ClaimsPrincipal user)
        {
            var sensor = db.Sensors.Find(sensorId);

            if (sensor != null && sensor.User == ClaimsHelper.GetUserEmail(user))
            {
                return sensor;
            }

            return null;
        }
    }
}
