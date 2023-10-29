using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Wiseflux.Models
{
    /// <summary>
    /// Available types for the sensor
    /// </summary>
    public enum EnumSensorType
    {
        /// <summary>
        /// Electricity sensor type
        /// </summary>
        Electricity = 0,
        /// <summary>
        /// Water sensor type
        /// </summary>
        Water = 1,
        /// <summary>
        /// Gas sensor type
        /// </summary>
        Gas = 2
    }

    /// <summary>
    /// Model atrelado ao sensor
    /// </summary>
    public class Sensor
    {
        /// <summary>
        /// Sensor id
        /// </summary>
        [Key]
        public int SensorId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [ForeignKey("FK_User")]
        public string User { get; set; }

        /// <summary>
        /// Sensor name
        /// </summary>
        [Required]
        public string SensorName { get; set; }

        /// <summary>
        /// Generated GUID in Sensor creation
        /// </summary>
        [Required]
        public Guid SensorGuid { get; set; }

        /// <summary>
        /// Sensor type
        /// </summary>
        [EnumDataType(typeof(EnumSensorType), ErrorMessage = "Please verify if you entered a valid role in `EnumSensorType` Scheme! Options = ['Electricity', 'Water', 'Gas']")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Required]
        public EnumSensorType SensorType { get; set; }

        /// <summary>
        /// Unit of the sensor
        /// </summary>
        /// <example>Electricity || Water || Gas</example>
        public string SensorUnit 
        { 
            get 
            { 
                if (SensorType == EnumSensorType.Electricity) { return "mV"; }
                else if (SensorType == EnumSensorType.Water) { return "L"; }
                else if (SensorType == EnumSensorType.Gas) { return "m³"; }
                return "none";
            } 
        }
    }
}
