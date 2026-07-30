using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MahantInv.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using static MahantInv.Infrastructure.Utility.Meta;

namespace MahantInv.Web.Areas.Identity.Pages.Account
{
    public class VerifyAuthenticatorModel : PageModel
    {
        private readonly UserManager<MIIdentityUser> _userManager;
        private readonly SignInManager<MIIdentityUser> _signInManager;
        [BindProperty]
        public string Code { get; set; }
        [BindProperty]
        public string UserName { get; set; }
        [BindProperty]
        public bool RememberMe { get; set; }

        public VerifyAuthenticatorModel(UserManager<MIIdentityUser> userManager, SignInManager<MIIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(UserName);
            }
            var isCodeValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, TokenOptions.DefaultAuthenticatorProvider, Code);

            if (!isCodeValid)
            {
                ModelState.AddModelError("Code", "Invalid code.");
                return Page();
            }

            user.IsMfaEnabled = true;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignInAsync(user, isPersistent: true);

            if (await _userManager.IsInRoleAsync(user, Roles.SellView))
            {
                return RedirectToAction("QuickSell", "Home");
            }
            return RedirectToPage("/Index");
        }
    }
}
