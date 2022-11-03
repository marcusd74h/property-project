using System.ComponentModel.DataAnnotations.Schema;

namespace AzureTest.Models.Entities
{
    public class FollowModel
    {
        public int Id { get; set; }
        public int CurrentUserId { get; set; }
        public int FollowingTargetId { get; set; }
        public DateTime Date { get; set; }
        [NotMapped]
        public UserModel User { get; set; } = new UserModel();
    }
}
