using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FunctionApp1
{
    public class Function
    {
        [FunctionName("Function")]
        public void Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var client = new SecretClient(vaultUri: new Uri("https://snehavault.vault.azure.net/"), credential: new DefaultAzureCredential());
            KeyVaultSecret secret = client.GetSecret("key1");
            log.LogInformation($"secret-Value:{secret.Value}");
            Console.WriteLine($"secret-Value:{secret.Value}");
        }
    }
}
