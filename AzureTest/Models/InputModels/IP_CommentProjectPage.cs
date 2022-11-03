namespace AzureTest.Models.InputModels
{
    public class IP_CommentProjectPage
    {
        public int ProjectPageId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
