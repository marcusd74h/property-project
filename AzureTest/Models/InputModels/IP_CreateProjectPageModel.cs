namespace AzureTest.Models.InputModels
{
    public class IP_CreateProjectPageModel
    {
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<string> TempImageIds { get; set; }
    }
}
