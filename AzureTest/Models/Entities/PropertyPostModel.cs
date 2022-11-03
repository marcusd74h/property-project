namespace AzureTest.Models.Entities
{
    public class PropertyPostModel
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public string Description { get; set; }
        public int PeopleVoted { get; set; }
        public int TotalStars { get; set; }
        public DateTime Date { get; set; }
        public int ImagesCount { get; set; }
        public int UserId { get; set; }
    }
}
