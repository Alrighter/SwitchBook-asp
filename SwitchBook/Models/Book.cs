namespace SwitchBook.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string OwnerId { get; set; }
    public User Owner { get; set; }
    public string Description { get; set; }

    public byte[] Image { get; set; }
}