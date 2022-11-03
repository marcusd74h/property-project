namespace AzureTest.Models.Entities
{
    public class ProjectPageCommentModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProjectPageId { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public UserModel User { get; set; } = new UserModel();
    }
}
