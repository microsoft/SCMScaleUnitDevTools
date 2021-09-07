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

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StorageAccountManager
    {
        private readonly string connectionString;

        public StorageAccountManager()
        {
            var scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            connectionString = scaleUnit.AzureStorageConnectionString;
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

            BlobClientOptions options = null;
            var sourceBlobClient = new BlobClient(sasUri, options);
            var blobContainerName = sourceBlobClient.BlobContainerName;
            var blobName = sourceBlobClient.Name;

            var targetBlobClient = new BlobClient(connectionString, blobContainerName, blobName);

            var operation = await targetBlobClient.StartCopyFromUriAsync(sasUri);

            await operation.WaitForCompletionAsync();

            Response response = await operation.UpdateStatusAsync();
            if (response.Status != 200)
            {
                throw new Exception($"\nAn error occured while copying the workloads to the blob: \n{response.Status}: {response.Content}");
            }
        }

        private async Task DeleteBlobContainers()
        {
            BlobServiceClient blobClient = new BlobServiceClient(connectionString);
            IEnumerable<BlobContainerItem> containers = blobClient.GetBlobContainers();

            if (containers is null || !containers.Any())
            {
                Console.WriteLine("No containers to be deleted\n");
                return;
            }

            foreach (var container in containers)
            {
                Console.WriteLine("Deleting Azure container: " + container.Name);
                await blobClient.DeleteBlobContainerAsync(container.Name);
            }
        }

        private async Task DeleteSharedTables()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            TableContinuationToken continuationToken = null;
            int count = 0;

            do
            {
                var listingResult = await tableClient.ListTablesSegmentedAsync(continuationToken);
                var tables = listingResult.Results.ToList();
                continuationToken = listingResult.ContinuationToken;
                foreach (var table in tables)
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
