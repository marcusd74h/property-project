namespace AzureTest.Models
{
    public class BlobInfo2
    {
        public BlobInfo2(Stream content, string contentType)
        {
            this.Content = content;
            this.ContentType = contentType;
        }
        public Stream Content { get; }
        public string ContentType { get; }
    }
}
