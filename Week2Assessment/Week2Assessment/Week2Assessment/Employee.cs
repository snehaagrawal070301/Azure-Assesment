using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Week2Assessment
{
    public class Employee
    {
        [Key]
        [JsonProperty("id")]
        public string employeeId { get; set; }
        public string employeeName { get; set;}
        public string role { get; set;}
    }
}
