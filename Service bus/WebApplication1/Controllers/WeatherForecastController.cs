using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Text;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController : ControllerBase
    {

        private readonly ILogger<EmployeeController> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly TableServiceClient _tableServiceClient;
        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
            _tableServiceClient = new TableServiceClient("DefaultEndpointsProtocol=https;AccountName=snehastorageac;AccountKey=q7NMj5nxqeeWr7ed235p79AiEFGzi6KT40Rq0qRHYOCSneENpapK/ONuuwouC1KFBSyHY+gbbIlw+AStZB7AQw==;EndpointSuffix=core.windows.net");
            _cosmosClient = new CosmosClient("AccountEndpoint=https://sneha-db.documents.azure.com:443/;AccountKey=Ki2yisH24eSH9LmeB4Ew8ysJE4kRSCaxnF1NFnWJu5Seaf7H8lm4kcgWYEI6E4UctA1PkXXLHtnrACDbZziawA==;");
        }

        [HttpPost(Name = "SendMessage")]
        public async Task<IActionResult> SendMessage(Employee employee)
        {
            try
            {
                await using var client = new ServiceBusClient("Endpoint=sb://sneha-serviceb-bus.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=hPI3yO0sD+p0w47qcS7/YMqhucNzTTOwj+ASbG5OmgY=;EntityPath=myqueue");
                var sender = client.CreateSender("myqueue");

                var messageBody = JsonConvert.SerializeObject(employee);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));
                await sender.SendMessageAsync(message);

                return Ok("Message sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet(Name = "ReadMessage")]
        public async Task<IActionResult> ReadMessage()
        {
            List<Employee> employees = new List<Employee>();
            try
            {
                await using var client = new ServiceBusClient("Endpoint=sb://sneha-serviceb-bus.servicebus.windows.net/;SharedAccessKeyName=send;SharedAccessKey=hPI3yO0sD+p0w47qcS7/YMqhucNzTTOwj+ASbG5OmgY=;EntityPath=myqueue");
                var receiver = client.CreateReceiver("myqueue");
                var database = _cosmosClient.GetDatabase("EmployeeDb");
                var container = database.GetContainer("container1");
                while (true) { 
                    ServiceBusReceivedMessage receivedMessage = await receiver.ReceiveMessageAsync();
                if (receivedMessage != null)
                {       string messageBody = Encoding.UTF8.GetString(receivedMessage.Body);
                        Console.WriteLine($"Received: {messageBody}");
                        Employee employee = JsonConvert.DeserializeObject<Employee>(messageBody);
                        var tableClient = _tableServiceClient.GetTableClient("Employee");
                        var tableEntity = new TableEntity("employeeId", Guid.NewGuid().ToString()) {
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
                             },
                        };
                        await tableClient.AddEntityAsync(tableEntity);
                        var response = await container.CreateItemAsync(employee, partitionKey: new PartitionKey(employee.employeeId));

                        if (response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            Console.WriteLine("Message pushed to Cosmos DB.");
                        }
                        else
                        {
                            Console.WriteLine("Failed to push message.");
                        }
                        employees.Add(employee);
                    
                    // Complete the message to remove it from the queue
                    await receiver.CompleteMessageAsync(receivedMessage);

                }
                else
                {
                    Console.WriteLine("No more messages in the queue.");
                        break;

                }

            }
                return Ok("Message sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

}
}