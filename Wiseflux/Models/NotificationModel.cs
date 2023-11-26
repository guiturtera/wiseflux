using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Wiseflux.Models
{
    public class NotificationModel
    {
        /// <summary>
        /// Sensor id
        /// </summary>
        [Key]
        public int NotificationId { get; set; }

        /// <summary>
        /// User id
        /// </summary>
        [ForeignKey("FK_User")]
        public string User { get; set; }

        /// <summary>
        /// Measure id
        /// </summary>
        [ForeignKey("FK_Measure")]
        public int MeasureId { get; set; }
    }
}
