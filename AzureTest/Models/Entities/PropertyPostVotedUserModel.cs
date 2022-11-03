namespace AzureTest.Models.Entities
{
    public class PropertyPostVotedUserModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Stars { get; set; }
        public int PropertyPostId { get; set; }
        public DateTime Date { get; set; }
    }
}
