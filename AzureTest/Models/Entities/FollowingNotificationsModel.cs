namespace AzureTest.Models.Entities
{
    public class FollowingNotificationsModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int TargetUserId { get; set; }
        public int Type { get; set; }
        public int ItemId{ get; set; }
    }
}
