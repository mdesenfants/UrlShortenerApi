using Microsoft.WindowsAzure.Storage.Table;

namespace UrlShortener
{
    public class ShortenedUrl : TableEntity
    {
        public ShortenedUrl()
        {

        }

        public ShortenedUrl(string source, string destination)
        {
            this.PartitionKey = source;
            this.RowKey = source;
            this.Source = source;
            this.Destination = destination;
        }

        public string Source { get; set; }
        public string Destination { get; set; }
    }
}
