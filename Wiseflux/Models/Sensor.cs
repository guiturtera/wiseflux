﻿using Microsoft.AspNetCore.Mvc;
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
        /// User id
        /// </summary>
        [ForeignKey("FK_User")]
        [Required]
        [Key]
        public string User { get; set; }
        
        /// <summary>
        /// Sensor id
        /// </summary>
        [Required]
        [Key]
        public string SensorId { get; set; }

        /// <summary>
        /// Generated GUID in Sensor creation
        /// </summary>
        [JsonIgnore]
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
        [Required]
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
