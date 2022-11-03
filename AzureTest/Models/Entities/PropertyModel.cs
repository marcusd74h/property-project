namespace AzureTest.Models.Entities
{
    public class PropertyModel
    {
        public int Id { get; set; }
        public int PropertyType { get; set; }
        public int AdminId { get; set; }
        public DateTime CreationDate { get; set; }
        public string PropertyName { get; set; }
        public int RegisterYear { get; set; }
        public int UploadsCount { get; set; }
        public string Description { get; set; }
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
