namespace AzureTest.Models.Entities
{
    public class NotificationConnectionModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationId { get; set; }
        public DateTime Date { get; set; }
        public NotificationModel Notification { get; set; }
    }
}
