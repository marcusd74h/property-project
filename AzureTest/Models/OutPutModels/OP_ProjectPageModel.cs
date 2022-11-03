using AzureTest.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureTest.Models.OutPutModels
{
    public class OP_ProjectPageModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        //public int TotalStars { get; set; }
        public int PeopleVotedNumber { get; set; }
        public int CommentsCount { get; set; }
        public int ImagesCount { get; set; }
        public bool UserHasSaved { get; set; }
        //public bool UserHasVoted { get; set; }
        //public int UserVotedStars { get; set; }
        public bool UserHasLiked { get; set; }
        public int DisplayImage { get; set; }
        public int CurrentImageNumber { get; set; }
        public int LoadedImagesNumber { get; set; }
        public int CurrentStackNumber { get; set; }
        //public string AverageRating { get; set; }
        public int TotalLikes { get; set; }
        public List<byte[]> Images { get; set; } = new List<byte[]>();
        public OP_UserModel User { get; set; } = new OP_UserModel();
        public OP_ProjectModel Project { get; set; } = new OP_ProjectModel();
    }
}
