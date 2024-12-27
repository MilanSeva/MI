using MahantInv.Infrastructure.Identity;
using MahantInv.Web.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MahantInv.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<MIIdentityUser> _userManager;
        private readonly SignInManager<MIIdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly GoogleCaptchaService _captchaService;

        public LoginModel(SignInManager<MIIdentityUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<MIIdentityUser> userManager, GoogleCaptchaService captchaService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _captchaService = captchaService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        //public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required, Display(Name = "Email/Username")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
            [Required]
            public string gRecaptchaResponse { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var captchaResult = await _captchaService.VerifyCaptchaAsync(Input.gRecaptchaResponse);

                //if (!captchaResult.Success || captchaResult.Score < 0.5)
                //{
                //    // Show visible reCAPTCHA v2 or reject the request
                //    return Page();
                //}
                MIIdentityUser identityUser;
                identityUser = await _userManager.FindByNameAsync(Input.Email);
                if (identityUser == null)
                {
                    identityUser = await _userManager.FindByEmailAsync(Input.Email);
                }
                if (identityUser == null || !await _userManager.CheckPasswordAsync(identityUser, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                //await _signInManager.SignInAsync(identityUser, Input.RememberMe);
                if (identityUser.IsMfaEnabled)
                {
                    return RedirectToPage("VerifyAuthenticator", new { UserName = identityUser.Email, Input.RememberMe });
                }
                else
                {
                    // Sign in user partially to access EnableAuthenticator page
                    return RedirectToPage("/Account/EnableAuthenticator", new { UserName = identityUser.Email });
                }
                //await _signInManager.SignInAsync(identityUser, Input.RememberMe);
                return RedirectToPage("/Index");

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(identityUser, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded && identityUser.IsMfaEnabled)
                {
                    return RedirectToAction("VerifyMfa");
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
