namespace AzureTest.Models.Entities
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public int TargetUserId { get; set; }
        public int NotificationType { get; set; }
        public int ItemId1 { get; set; }
        public int ItemId2 { get; set; }
        public string ItemText1 { get; set; }
        public string ItemText2 { get; set; }
        public DateTime Date { get; set; }

        //DONE 1 = project page upload; ItemId1 = ProjectId, ItemId2 = UserId, ItemText1 = ProjectName, ItemText2 = Firstname + Lastname

        //DONE 2 = user started to follow you; ItemId1 = UserId, ItemId2 = null, ItemText1 = Firstname + Lastname, ItemText2 = null 

        //DONE 3 = user created a new project; ItemId1 = ProjectId, ItemId2 = UserId, ItemText1 = ProjectName, ItemText2 = FirstName + Lastname

        // 4 = user commented on your project page; IdemId1 = ProjectPageId, ItemId2 = UserId, ItemText1 = ProjectName, ItemText1 = Firstname + Lastname
    }
}
