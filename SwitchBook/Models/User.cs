using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SwitchBook.Models
{
    public class User : IdentityUser
    {
        public byte[] Avatar { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}