using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OpenWhistle.Pages;

public class LoginModel : PageModel
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public LoginModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; }

    public string ErrorMessage { get; set; } //FIXME:

    public class LoginInputModel
    {
        [Required]
        [StringLength(256)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Login Failed.";
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        ErrorMessage = result.IsLockedOut ? "Login Failed. You're currently locked out. Please try again later." : "Login Failed. Wrong credentials. Please try again. "; 

        return Page();
    }
}