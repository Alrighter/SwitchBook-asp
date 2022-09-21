using Microsoft.AspNetCore.Identity;

namespace SwitchBook.Models
{
    public class User : IdentityUser
    {
        public byte[] Avatar { get; set; }
        public int AddressId { get; set; }
    }
}