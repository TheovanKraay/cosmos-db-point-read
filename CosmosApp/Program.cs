using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace CosmosApp
{
    class Program
    {
        private static readonly string containerId = "Item";
        private static readonly string databaseId = "Movies";

        private static readonly string endpoint = "endpoint";
        private static readonly string authKey = "authKey";

        private static Database database = null;
        private static Container container = null;
        static async Task Main(string[] args)
        {



            using (CosmosClient client = new CosmosClient(endpoint, authKey))

            {

                await Program.Initialize(client);

                Console.WriteLine("Hello World!");
                Console.WriteLine("\n1.2 - Reading Item by Id");
                // Note that Reads require a partition key to be specified.
                await Program.ReadItemAsync();

            }

        }
        private static async Task ReadItemAsync()
        {
            try
            {
                ItemResponse<T> response = await container.ReadItemAsync<T>(
                partitionKey: new PartitionKey("Category"), id: "a88b95ec-822c-4a5d-93f2-69465723f353");

                Console.WriteLine($"Request charge: {response.RequestCharge}");

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e);
            }

        }


        private static async Task Initialize(CosmosClient client)

        {
            database = await client.CreateDatabaseIfNotExistsAsync(databaseId);
            container = await database.CreateContainerIfNotExistsAsync(containerId, "/partitionKey");
        }
        private class T
        {
        }
    }
}
