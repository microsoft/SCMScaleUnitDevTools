using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class BlobCleaner
    {
        private string connectionString;

        public async Task CleanBlob()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            connectionString = scaleUnit.AzureStorageConnectionString;

            await DeleteBlobContainers();

            await DeleteSharedTablesAsync();
        }

        private async Task DeleteBlobContainers()
        {
            BlobServiceClient blobClient = new BlobServiceClient(connectionString);
            IEnumerable<BlobContainerItem> containers = blobClient.GetBlobContainers();

            if (!(bool)(containers?.Any()))
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

        private async Task DeleteSharedTablesAsync()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            TableContinuationToken continuationToken = null;
            var allTables = new List<CloudTable>();

            do
            {
                var listingResult = await tableClient.ListTablesSegmentedAsync(continuationToken);
                var tables = listingResult.Results.ToList();
                continuationToken = listingResult.ContinuationToken;
                foreach (var table in tables)
                {
                    allTables.Add(table);
                }
            }
            while (continuationToken != null);

            if (!(bool)allTables?.Any())
            {
                Console.WriteLine("No tables to be deleted\n");
                return;
            }

            foreach (var table in allTables)
            {
                Console.WriteLine("Deleting Azure table: " + table.Name);
                await table.DeleteIfExistsAsync();
            }
        }
    }
}
