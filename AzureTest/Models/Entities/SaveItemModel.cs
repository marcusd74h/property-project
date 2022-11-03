using System.ComponentModel.DataAnnotations.Schema;

namespace AzureTest.Models.Entities
{
    public class SaveItemModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        [NotMapped]
        public ProjectPageModel ProjectPage { get; set; } = new ProjectPageModel();
        [NotMapped]
        public FollowModel Follows { get; set; } = new FollowModel();
    }
}
