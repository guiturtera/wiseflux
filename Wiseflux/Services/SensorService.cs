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


        public async Task<ServiceResponse<object>> AddSensor(Sensor newSensor, ClaimsPrincipal newUser)
        {
            //User existingUser;
            //if (UserExistsByEmail(newUser.Email, out existingUser))
            //    return new ServiceResponse<object>(HttpStatusCode.Conflict, "User already exists", null);

            //newUser.Password = new UserSecurity().EncryptPassword(newUser.Password);
            
            newSensor.SensorGuid= Guid.NewGuid();

            _db.Sensors.Add(newSensor);
            _db.SaveChanges();

            return new ServiceResponse<object>(HttpStatusCode.OK, "Success", null);
        }
    }
}
