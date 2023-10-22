using System.Net;
using System.Text.Json.Serialization;

namespace Wiseflux.Models
{
    public class ServiceResponse<T>
    {
        public ServiceResponse (HttpStatusCode status, string message, T response) 
        {
            Status = status;
            Message = message;
            Response = response;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HttpStatusCode? Status { get; set; }
        public string? Message { get; set; }
        public T? Response { get; set; }
    }
}
