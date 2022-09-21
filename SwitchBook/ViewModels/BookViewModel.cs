using Microsoft.AspNetCore.Http;

namespace SwitchBook.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int OwnerId { get; set; }
        public string Description { get; set; }

        public IFormFile Image { get; set; }
    }
}