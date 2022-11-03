using System.ComponentModel.DataAnnotations.Schema;

namespace AzureTest.Models.Entities
{
    public class ProjectPageModel
    {
        //public ProjectPageModel()
        //{
        //    Project = new ProjectModel();
        //}
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        //public int TotalStars { get; set; }
        //public int PeopleVotedNumber { get; set; }
        public int TotalLikes { get; set; }
        public int CommentsCount { get; set; }
        public int ImagesCount { get; set; }
        [NotMapped]
        public SaveItemModel UserHasSaved { get; set; }
        [NotMapped]
        public LikeProjectPageModel UserHasLiked { get; set; }
        [NotMapped]
        public FollowModel UserFollowsUser { get; set; }
        [NotMapped]
        public string ProjectName { get; set; }
        [NotMapped]
        public ProjectModel Project { get; set; } = new ProjectModel();
        [NotMapped]
        public UserModel User { get; set; }
    }
}
