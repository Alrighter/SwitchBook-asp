namespace SwitchBook.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Region { get; set; }

        public string City { get; set; }

        public string Street { get; set; }
        public string PostalCode { get; set; }

        public string PhoneNumber { get; set; }
    }
}