using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MahantInv.Infrastructure.Identity;

namespace MahantInv.Web.Areas.Identity.Pages.Account
{
    public class VerifyAuthenticatorModel : PageModel
    {
        private readonly UserManager<MIIdentityUser> _userManager;

        [BindProperty]
        public string Code { get; set; }

        public VerifyAuthenticatorModel(UserManager<MIIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, TokenOptions.DefaultAuthenticatorProvider, Code);

            if (!isCodeValid)
            {
                ModelState.AddModelError("Code", "Invalid code.");
                return Page();
            }

            user.IsMfaEnabled = true;
            await _userManager.UpdateAsync(user);

            return RedirectToPage("/Index");
        }
    }
}
