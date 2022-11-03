namespace AzureTest.Models.Entities
{
    public class ProjectPageVoteModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int ProjectPageId { get; set; }
        public int UserId { get; set; }
        public int StarsCount { get; set; }
        public DateTime Date { get; set; }
    }
}
