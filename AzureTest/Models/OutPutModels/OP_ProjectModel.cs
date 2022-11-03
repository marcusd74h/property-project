namespace AzureTest.Models.OutPutModels
{
    public class OP_ProjectModel
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public int Status { get; set; }
        public int UpdatesCount { get; set; }
        public byte[] ProjectProfileImage { get; set; }
        public int PeopleVoted { get; set; }
        public int CommentsCount { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public List<OP_RenderDatesModel> RenderDates { get; set; } = new List<OP_RenderDatesModel>();
        public OP_UserModel User { get; set; }
    }
}
