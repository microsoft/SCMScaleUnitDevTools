using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ScaleUnitManagement.Utilities;
using System;

namespace ScaleUnitManagement.DatabaseManager
{
    public class StorageAccountManager
    {
        private readonly string connectionString;

        public StorageAccountManager()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            connectionString = scaleUnit.AzureStorageConnectionString;
            if (string.IsNullOrEmpty(connectionString) && scaleUnit.EnvironmentType == EnvironmentType.LCSHosted)
            {
                using (var webConfig = new WebConfig(decrypt: true))
                {
                    connectionString = webConfig.GetXElementValue("AzureStorage.StorageConnectionString");
                }
            }
        }

        public async Task CleanStorageAccount()
        {
            await DeleteBlobContainers();
            await DeleteSharedTables();
        }

        public async Task ImportWorkloadsBlob(string sasUrlString)
        {
            Uri sasUri;
            try
            {
                sasUri = new Uri(sasUrlString);
            }
            catch (Exception)
            {
                throw new Exception($"{sasUrlString} is not a valid Uri");
            }

            string blobContainerName = "sysworkloadinstancesharedserviceunitstorage";
            string blobName = "current.json";

            var targetBlobClient = new BlobClient(connectionString, blobContainerName, blobName);

            var blobUriBuilder = new BlobUriBuilder(sasUri)
            {
                BlobName = blobName
            };
            var blobUri = blobUriBuilder.ToUri();


            CopyFromUriOperation operation = await targetBlobClient.StartCopyFromUriAsync(blobUri);

            await operation.WaitForCompletionAsync();

            Response response = await operation.UpdateStatusAsync();
            if (response.Status != 200)
            {
                throw new Exception($"\nAn error occured while copying the workloads to the blob: \n{response.Status}: {response.Content}");
            }
        }

        private async Task DeleteBlobContainers()
        {
            var blobClient = new BlobServiceClient(connectionString);
            IEnumerable<BlobContainerItem> containers = blobClient.GetBlobContainers();

            if (containers is null || !containers.Any())
            {
                Console.WriteLine("No containers to be deleted\n");
                return;
            }

            foreach (BlobContainerItem container in containers)
            {
                Console.WriteLine("Deleting Azure container: " + container.Name);
                await blobClient.DeleteBlobContainerAsync(container.Name);
            }
        }

        private async Task DeleteSharedTables()
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            TableContinuationToken continuationToken = null;
            int count = 0;

            do
            {
                TableResultSegment listingResult = await tableClient.ListTablesSegmentedAsync(continuationToken);
                var tables = listingResult.Results.ToList();
                continuationToken = listingResult.ContinuationToken;
                foreach (CloudTable table in tables)
                {
                    count++;
                    Console.WriteLine("Deleting Azure table: " + table.Name);
                    await table.DeleteIfExistsAsync();
                }
            }
            while (continuationToken != null);

            if (count == 0)
            {
                Console.WriteLine("No tables to be deleted\n");
                return;
            }
        }
    }
}
