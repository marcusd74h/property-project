namespace AzureTest.Models.OutPutModels
{
    public class OP_HomePage
    {
        public OP_UserModel User { get; set; }
        public List<OP_ProjectPageModel> Projects { get; set; } = new List<OP_ProjectPageModel>();
    }
}
