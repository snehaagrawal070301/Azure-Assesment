using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.Text;

namespace Week2Assessment.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly TableServiceClient _tableServiceClient;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
            _tableServiceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=snehastorageacc;AccountKey=j3n81eJzMboyAzwoojmm24nPfINFYzeRQ1HvKYMB6S1i9m+dxI3p9A1uV9nk1/fmR2kjYyVdPA3k+AStd3vfhQ==;EndpointSuffix=core.windows.net");
            _cosmosClient = new CosmosClient("AccountEndpoint=https://sneha-cosmos-db.documents.azure.com:443/;AccountKey=kVf4ypBA9PGxOt9GGdRVeBgoWdV2JDNlRLE1USNGWdRJv6rRypcU15EfvPPNCTLBviUE7vr39YXsACDbNvm78g==;");
        }
        [HttpPost(Name = "SendMessage")]
        public async Task<IActionResult> SendMessage(Employee employee)
        {
            try
            {
                await using var client = new ServiceBusClient("Endpoint=sb://sneha-service-bus.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=71nd5VbaHU8Rbkjxx+fW36Ugu+WI3wLtz+ASbDXZlbU=");
                var sender = client.CreateSender("Queue1");
                var messageBody = JsonConvert.SerializeObject(employee);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));
                await sender.SendMessageAsync(message);
                return Ok("Message sent successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet(Name = "ReadMessage")]
        public async Task<IActionResult> ReadMessage()
        {
            try
            {
                await using var client = new ServiceBusClient("Endpoint=sb://sneha-service-bus.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=71nd5VbaHU8Rbkjxx+fW36Ugu+WI3wLtz+ASbDXZlbU=");
                var receiver = client.CreateReceiver("Queue1");
                var database = _cosmosClient.GetDatabase("EmployeeDb");
                var container = database.GetContainer("container1");
                while(true)
                {
                    ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
                    if(receivedMessage != null)
                    {
                        var messageBody = Encoding.UTF8.GetString(receivedMessage.Body);
                        Console.WriteLine($"Received: {messageBody}");
                        Employee employee = JsonConvert.DeserializeObject<Employee>(messageBody);
                        var tableClient = _tableServiceClient.GetTableClient("Employee");
                        var tableEntity = new TableEntity("employeeId", Guid.NewGuid().ToString())
                        {
                            {
                                "employeeId",
                                employee.employeeId
                            },
                            {
                                "employeeName",
                                employee.employeeName
                            },
                            {
                                "role",
                                employee.role
                            }
                        };
                        await tableClient.AddEntityAsync(tableEntity);
                        var response = await container.CreateItemAsync(employee, partitionKey: new PartitionKey(employee.employeeId));
                        if(response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            Console.WriteLine("Message Pushed to cosmosDb");
                        }
                        else
                        {
                            Console.WriteLine("Failed to push message");
                        }
                        await receiver.CompleteMessageAsync(receivedMessage);
                    }
                    else
                    {
                        Console.WriteLine("No More message in queue");
                        break;
                    }
                }
                return Ok("Message receive successfully");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
