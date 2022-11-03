using AzureTest.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace AzureTest.Models
{
    public class UserModel : IdentityUser<int>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime RegisterDate { get; set; }
        public string Description { get; set; }
        public int RegisterYear { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int AccountType { get; set; }
        [NotMapped]
        public List<ProjectModel> Projects { get; set; } = new List<ProjectModel>();
        [NotMapped]
        public List<UserModel> Followers { get; set; } = new List<UserModel>();
    }
}
