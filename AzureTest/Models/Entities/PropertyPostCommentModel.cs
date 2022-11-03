namespace AzureTest.Models.Entities
{
    public class PropertyPostCommentModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
