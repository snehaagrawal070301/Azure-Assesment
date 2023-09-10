using Azure.Data.Tables;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplication1
{
    public class Employee
    {
        [Key]
        [JsonProperty("id")]
        public string employeeId { get; set; }
        public string employeeName { get; set; }
        public string role { get; set; }
    }
}
