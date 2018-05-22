using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener
{
    public static class Store
    {
        private static CloudTable Shortened
        {
            get
            {
                var conn = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
                var storageAccount = CloudStorageAccount.Parse(conn);
                var tableClient = storageAccount.CreateCloudTableClient();
                var table = tableClient.GetTableReference("shortened");
                table.CreateIfNotExists();
                return table;
            }
        }

        public static bool IsValidSource(string source)
        {
            return !(string.IsNullOrWhiteSpace(source) || source.Length > 200 || source.Any(c => !char.IsLetterOrDigit(c)));
        }

        public async static Task<bool> Exists(string source)
        {
            if (!IsValidSource(source))
            {
                return false;
            }

            var operation = TableOperation.Retrieve<ShortenedUrl>(source, source);
            var result = await Shortened.ExecuteAsync(operation);

            if (result.Result != null)
            {
                return true;
            }

            return false;
        }

        public async static Task<Uri> GetDestination(string source)
        {
            if (!IsValidSource(source))
            {
                return null;
            }

            var operation = TableOperation.Retrieve<ShortenedUrl>(source, source);
            var result = await Shortened.ExecuteAsync(operation);
            if (result.Result != null)
            {
                return new Uri((result.Result as ShortenedUrl)?.Destination);
            }

            return null;
        }

        public async static Task SaveRoute(string source, Uri destination)
        {
            if (!IsValidSource(source))
            {
                throw new ArgumentException(source);
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            var operation = TableOperation.Insert(new ShortenedUrl(source, destination.ToString()));
            await Shortened.ExecuteAsync(operation);
        }
    }
}