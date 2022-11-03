namespace AzureTest.Models.OutPutModels
{
    public class OP_UserModel
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Description { get; set; }
        public int UploadsCount { get; set; }
        public int YearsOnProperty { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public int NotificationCount { get; set; }
        public bool FollowsUser { get; set; }
        public byte[] ProfileImage { get; set; }
        public int AccountType { get; set; }
        public List<OP_NotificationModel> Notifications { get; set; } = new List<OP_NotificationModel>();
        public List<OP_ProjectModel> Projects { get; set; } = new List<OP_ProjectModel>();
    }
}
