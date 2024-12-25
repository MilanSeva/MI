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

    public class EnableAuthenticatorModel : PageModel
    {
        private readonly UserManager<MIIdentityUser> _userManager;

        public string QrCodeImage { get; set; }
        public string SharedKey { get; set; }

        public EnableAuthenticatorModel(UserManager<MIIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

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

        private string GenerateQrCode(string email, string key)
        {
            var qrCodeUri = $"otpauth://totp/MyApp:{email}?secret={key}&issuer=MyApp&digits=6";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);

            return $"data:image/png;base64,{Convert.ToBase64String(qrCode.GetGraphic(20))}";
        }
    }

}
