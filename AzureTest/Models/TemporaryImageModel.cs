namespace AzureTest.Models
{
    public class TemporaryImageModel
    {
        public int Id { get; set; }
        public int ImageType { get; set; }
        public byte[] Image { get; set; }
        public DateTime Date { get; set; }
        public int ItemId { get; set; }
    }
}
