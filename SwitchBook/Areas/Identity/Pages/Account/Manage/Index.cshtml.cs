using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SwitchBook.Data;
using SwitchBook.Models;

namespace SwitchBook.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    private InputModel oldVersion;

    public IndexModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = context;
    }

    public string Username { get; set; }

    [TempData] public string StatusMessage { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    private async Task LoadAsync(User user)
    {
        var userName = await _userManager.GetUserNameAsync(user);

        var address = await _db.Address.FirstOrDefaultAsync(x => x.UserId == user.Id);
        
        Username = userName;
        if (address != null)
        {
            Input = new InputModel
            {
                UserName = userName,
                PhoneNumber = address.PhoneNumber,
                Street = address.Street,
                City = address.City,
                Region = address.Region,
                PostalCode = address.PostalCode
            };
            oldVersion = (InputModel)Input.Clone();
        }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }


        if (Input != null)
        {
            var address = await _db.Address.FirstOrDefaultAsync(x => x.UserId == user.Id);

            address.City = Input.City;
            address.Region = Input.Region;
            address.PostalCode = Input.PostalCode;
            address.PhoneNumber = Input.PhoneNumber;
            address.Street = Input.Street;


            _db.Address.Update(address);
            await _db.SaveChangesAsync();
        }

        //var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        //if (Input.PhoneNumber != phoneNumber)
        //{
        //    var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
        //    if (!setPhoneResult.Succeeded)
        //    {
        //        StatusMessage = "Unexpected error when trying to set phone number.";
        //        return RedirectToPage();
        //    }
        //}
        var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
        if (!setUserNameResult.Succeeded)
        {
            StatusMessage = "Неочікувана помилка при спробі задати ім'я користувача.";
            return RedirectToPage();
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Ваш профіль оновлено";
        return RedirectToPage();
    }

    public class InputModel : ICloneable
    {
        [Display(Name = "User Name")] public string UserName { get; set; }

        [Display(Name = "Region")] public string Region { get; set; }

        [Display(Name = "City")] public string City { get; set; }

        [Display(Name = "Street")] public string Street { get; set; }

        [Display(Name = "PostalCode")] public string PostalCode { get; set; }

        [Display(Name = "PhoneNumber")]
        [Phone]
        public string PhoneNumber { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}