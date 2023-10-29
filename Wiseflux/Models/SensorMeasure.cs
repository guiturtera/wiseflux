using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Wiseflux.Models
{
    /// <summary>
    /// Model used to measure sensor metrics
    /// </summary>
    public class SensorMeasure
    {
        /// <summary>
        /// Measure Id (unit specified by the sensor)
        /// </summary>
        [JsonIgnore]
        [Key]
        public int MeasureId { get; set; }

        /// <summary>
        /// Sensor Id
        /// </summary>
        [ForeignKey("Sensor")]
        [Required]
        [JsonIgnore]
        public int SensorId { get; set; }

        /// <summary>
        /// Measure value (unit specified by the sensor)
        /// </summary>
        [Required]
        public double MeasureValue { get; set; }

        /// <summary>
        /// UTC DateTime of the measure
        /// </summary>
        [Required]
        public DateTime MeasureTime { get; set; }
    }
}
