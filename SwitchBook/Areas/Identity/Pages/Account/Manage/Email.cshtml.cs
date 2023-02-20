using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SwitchBook.Models;

namespace SwitchBook.Areas.Identity.Pages.Account.Manage;

public class EmailModel : PageModel
{
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public EmailModel(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public string Username { get; set; }

    public string Email { get; set; }

    public bool IsEmailConfirmed { get; set; }

    [TempData] public string StatusMessage { get; set; }

    public string Link { get; set; }

    [BindProperty] public InputModel Input { get; set; }

    private async Task LoadAsync(User user)
    {
        var email = await _userManager.GetEmailAsync(user);
        Email = email;

        Input = new InputModel
        {
            NewEmail = email
        };

        IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostChangeEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var email = await _userManager.GetEmailAsync(user);
        if (Input.NewEmail != email)
        {
            var newEmail = Input.NewEmail;
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);

            //change email
            var result = await _userManager.ChangeEmailAsync(user, newEmail, code);
            if (!result.Succeeded)
            {
                StatusMessage = "Електронну пошту не змінено.";
            }
            else
            {
                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Електронну пошту змінено успішно.";
                return RedirectToPage();
            }


            return RedirectToPage();
        }

        StatusMessage = "Ваша електронна пошта не змінилася.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendVerificationEmailAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var userId = await _userManager.GetUserIdAsync(user);
        var email = await _userManager.GetEmailAsync(user);
        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //var callbackUrl = Url.Page(
        //    "/Account/ConfirmEmail",
        //    pageHandler: null,
        //    values: new { area = "Identity", userId = userId, code = code },
        //    protocol: Request.Scheme);
        //await _emailSender.SendEmailAsync(
        //    email,
        //    "Confirm your email",
        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //change email without confirmaion
        var result = await _userManager.ChangeEmailAsync(user, email, email);
        if (!result.Succeeded)
        {
            userId = await _userManager.GetUserIdAsync(user);
            throw new InvalidOperationException(
                $"Unexpected error occurred setting email for user with ID '{userId}'.");
        }

        await _signInManager.RefreshSignInAsync(user);

        StatusMessage = "Електронну пошту змінено успішно.";

        return RedirectToPage();
    }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New email")]
        public string NewEmail { get; set; }
    }
}