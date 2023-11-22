using System.Globalization;
using System.Net;
using System.Security.Claims;
using Wiseflux.Data;
using Wiseflux.Helpers;
using Wiseflux.Models;
using Wiseflux.Security;

namespace Wiseflux.Services
{
    public class SensorMeasureService
    {
        private readonly ApplicationDbContext _db;

        public SensorMeasureService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResponse<SensorMeasure>> AddMeasure(SensorMeasure sensorMeasure, int sensorId, Guid sensorGuid)
        {
            Sensor sensor = _db.Sensors.Find(sensorId);
            if (sensor == null || sensor.SensorGuid != sensorGuid) 
            { 
                return new ServiceResponse<SensorMeasure>(HttpStatusCode.NotFound, "Sensor not found.", null);
            }

            sensorMeasure.SensorId = sensorId;
            
            _db.SensorMeasures.Add(sensorMeasure);
            _db.SaveChanges();

            return new ServiceResponse<SensorMeasure>(HttpStatusCode.OK, "Success", sensorMeasure);
        }

        public async Task<ServiceResponse<List<SensorMeasure>>> GetSensorMeasures(int sensorId, DateTime startDate, DateTime endDate, ClaimsPrincipal user)
        {
            var sensor = SensorHelper.GetSensorFromUser(_db, sensorId, user);

            if (sensor == null) { return new ServiceResponse<List<SensorMeasure>>(HttpStatusCode.NotFound, "Sensor not found.", null); }

            DateTime formattedStartDate = startDate; // if not specified comes as min value
            DateTime formattedEndDate = endDate;

            if (formattedEndDate == DateTime.MinValue) formattedEndDate = DateTime.UtcNow; // if not specified (min value) returns all measures until the current datetime

            var measures = _db.SensorMeasures.Where(measure =>
                measure.SensorId == sensorId &&
                measure.MeasureTime >= formattedStartDate &&
                measure.MeasureTime <= formattedEndDate
            ).ToList();

            return new ServiceResponse<List<SensorMeasure>>(HttpStatusCode.OK, "Sucesso", measures);
        }

        private async Task<ServiceResponse<List<SensorMeasure>>> getAvarageDaysByFormat(int sensorId, DateTime startDate, DateTime endDate, ClaimsPrincipal user, string format)
        {
            var sensor = SensorHelper.GetSensorFromUser(_db, sensorId, user);

            if (sensor == null) { return new ServiceResponse<List<SensorMeasure>>(HttpStatusCode.NotFound, "Sensor not found.", null); }

            DateTime formattedStartDate = startDate; // if not specified comes as min value
            DateTime formattedEndDate = endDate;

            if (formattedEndDate == DateTime.MinValue) formattedEndDate = DateTime.UtcNow; // if not specified (min value) returns all measures until the current datetime

            List<SensorMeasure> measures = _db.SensorMeasures.Where(measure =>
                measure.SensorId == sensorId &&
                measure.MeasureTime >= formattedStartDate &&
                measure.MeasureTime <= formattedEndDate
            ).ToList();

            dynamic res = measures.GroupBy(measure =>
                measure.MeasureTime.ToString(format)
            ).Select(a =>
                new SensorMeasure { MeasureValue = a.Average(a => a.MeasureValue), MeasureTime = DateTime.ParseExact(a.Key, format, CultureInfo.CurrentCulture) }
            ).ToList();

            return new ServiceResponse<List<SensorMeasure>>(HttpStatusCode.OK, "Sucesso", res);
        }

        public async Task<ServiceResponse<List<SensorMeasure>>> GetAvarageSensorMeasuresByMonth(int sensorId, DateTime startDate, DateTime endDate, ClaimsPrincipal user)
        {
            return await getAvarageDaysByFormat(sensorId, startDate, endDate, user, "MM/yy");
        }

        public async Task<ServiceResponse<List<SensorMeasure>>> GetAvarageSensorMeasuresByDay(int sensorId, DateTime startDate, DateTime endDate, ClaimsPrincipal user)
        {
            return await getAvarageDaysByFormat(sensorId, startDate, endDate, user, "dd/MM/yy");
        }
    }
}
