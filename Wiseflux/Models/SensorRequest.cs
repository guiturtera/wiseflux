using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Wiseflux.Models
{
    public class SensorRequest
    {
        /// <summary>
        /// Sensor name
        /// </summary>
        [Required]
        public string SensorName { get; set; }

        /// <summary>
        /// Sensor type
        /// </summary>
        [EnumDataType(typeof(EnumSensorType), ErrorMessage = "Please verify if you entered a valid role in `EnumSensorType` Scheme! Options = ['Electricity', 'Water', 'Gas']")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required]
        public EnumSensorType SensorType { get; set; }
    }
}
