namespace AzureTest.Models.InputModels
{
    public class IP_CreateProjectModel
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
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
    }
}
