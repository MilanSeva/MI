namespace MahantInv.Web.Areas.Identity.Pages.Account
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using QRCoder;
    using QRCoder.Core;
    using System.Threading.Tasks;
    using System;
    using MahantInv.Infrastructure.Identity;
    using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

    public class EnableAuthenticatorModel : PageModel
    {
        private readonly UserManager<MIIdentityUser> _userManager;

        public string QrCodeImage { get; set; }
        public string SharedKey { get; set; }
        [BindProperty]
        public string Code { get; set; }
        [BindProperty]
        public string UserName { get; set; }

        public EnableAuthenticatorModel(UserManager<MIIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userName)
        {
            UserName = userName;
            var user = await _userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(UserName);
            }

            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = authenticatorKey;
            QrCodeImage = GenerateQrCode(user.Email, authenticatorKey);

            return Page();
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
                var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(authenticatorKey))
                {
                    await _userManager.ResetAuthenticatorKeyAsync(user);
                    authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
                }

                SharedKey = authenticatorKey;
                QrCodeImage = GenerateQrCode(user.Email, authenticatorKey);
                return Page();
            }

            user.IsMfaEnabled = true;
            await _userManager.UpdateAsync(user);

            return RedirectToPage("/Account/Login");
        }

        private string GenerateQrCode(string email, string key)
        {
            var qrCodeUri = $"otpauth://totp/Mahant Kothar:{email}?secret={key}&issuer=Mahant Kothar&digits=6";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            return $"data:image/png;base64,{Convert.ToBase64String(qrCode.GetGraphic(20))}";
        }
    }

}
