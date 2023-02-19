namespace SwitchBook.Models
{
    public class Order
    {
        public int Id { get; set; }
        
        public int FirstBookId { get; set; }
        
        public int LastBookId { get; set; }
        
        public int FirstAddressId { get; set; }

        public int LastAddressId { get; set; }

        public bool IsConfirm { get; set; }
    }
}