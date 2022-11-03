using System.ComponentModel.DataAnnotations.Schema;

namespace AzureTest.Models.Entities
{
    public class ProjectModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public int Status { get; set; }
        public int UpdatesCount { get; set; }
        public int CommentsCount { get; set; }
        public int VotesCount { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string county { get; set; }
        public string state { get; set; }
        public string country_code { get; set; }
        public string formatted { get; set; }
        public string lat { get; set; }
        public string lon { get; set; }
        public string postcode { get; set; }
        public string street { get; set; }
        [NotMapped]
        public FollowModel FollowsUser { get; set; }
        [NotMapped]
        public UserModel User { get; set; } = new UserModel();
        [NotMapped]
        public List<ProjectPageModel> ProjectPages { get; set; } = new List<ProjectPageModel>();


    }
}
