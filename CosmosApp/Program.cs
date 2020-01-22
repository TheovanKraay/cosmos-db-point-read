using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosApp
{
    class Program
    {
        private static readonly string containerId = "test";
        private static readonly string databaseId = "testdb";

        private static readonly string endpoint = "endpoint";
        private static readonly string authKey = "authKey";

        private static Database database = null;
        private static Container container = null;
        static async Task Main(string[] args)
        {
            using (CosmosClient client = new CosmosClient(endpoint, authKey))
            {
                await Program.Initialize(client);
                Console.WriteLine("Getting read and write charges...");
                // Note that Reads require a partition key to be specified.
                await Program.ReadItemAsync();
            }
        }
        private static async Task ReadItemAsync()
        {
            try
            {
                ItemResponse<T> response = await container.ReadItemAsync<T>(
                partitionKey: new PartitionKey("pk1"), id: "b87293f2-fb70-4e20-a913-a270f6d6f9ac");

                Console.WriteLine($"Read Request charge: {response.RequestCharge}");

                var json = "[{\"id\": \"" + Guid.NewGuid() + "\",\"pk\": \"pk1\",\"Name\": \"theo\"}," +
                            "{\"id\": \"" + Guid.NewGuid() + "\",\"pk\": \"pk1\",\"Name\": \"theo\",\"Surname\": \"van Kraay\"}," +
                            "{\"id\": \"" + Guid.NewGuid() + "\",\"pk\": \"pk1\",\"Name\": \"theo\",\"Surname\": \"van Kraay\",\"Address\": \"Somewhere\"}," +
                            "{\"id\": \"" + Guid.NewGuid() + "\",\"pk\": \"pk1\",\"Name\": \"theo\",\"Surname\": \"van Kraay\",\"Address\": \"Somewhere\",\"Postcode\": \"FY8353\"}," +
                            "{\"id\": \"" + Guid.NewGuid() + "\",\"pk\": \"pk1\",\"Name\": \"theo\",\"Surname\": \"van Kraay\",\"Address\": \"Somewhere\",\"Postcode\": \"FY8353\",\"PhoneNumber\": \"98927267728\"}]";

                var docs = JsonConvert.DeserializeObject<List<JObject>>(json);

                foreach (var doc in docs)
                {
                    ItemResponse<JObject> response2 =  await container.CreateItemAsync<JObject>(doc, new PartitionKey(doc.GetValue("pk").ToString()));
                    Console.WriteLine($"Write Request charge: {response2.RequestCharge}");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }

        }


        private static async Task Initialize(CosmosClient client)

        {
            database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/pk");
        }
        private class T
        {
        }
    }
}
