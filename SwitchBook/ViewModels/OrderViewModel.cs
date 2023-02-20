namespace SwitchBook.ViewModels;

public class OrderViewModel
{
    public int FirstBookId { get; set; }
    public int LastBookId { get; set; }

    //Address
    public string Region     { get; set; }

    public string City          { get; set; }

    public string Street       { get; set; }
    public string PostalCode   { get; set; }

    public string PhoneNumber   { get; set; }
}