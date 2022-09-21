using System.Collections.Generic;
using SwitchBook.Models;

namespace SwitchBook.ViewModels
{
    public class TradeViewModel
    {
        public User Owner { get; set; }
        public Book Book { get; set; }

        public List<Book> MyBooks { get; set; }
    }
}