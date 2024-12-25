using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QRCoder.Core;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Drawing.Imaging;

namespace MahantInv.Web.Controllers
{
    public class TwoFactorController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public TwoFactorController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GenerateQrCode()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Generate a new authenticator key if not already set
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // Generate QR Code URI
            var uri = GenerateAuthenticatorUri(user.Email, key);

            // Generate QR Code Image
            var qrCodeImage = GenerateQrCode(uri);

            return File(qrCodeImage, "image/png");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyToken([FromBody] string token)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, token);

            if (!isValid)
            {
                return BadRequest("Invalid token.");
            }

            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);

            return Ok("Two-factor authentication enabled.");
        }

        private string GenerateAuthenticatorUri(string email, string key)
        {
            return $"otpauth://totp/MyApp:{Uri.EscapeDataString(email)}?secret={Uri.EscapeDataString(key)}&issuer=MyApp&digits=6";
        }

        private byte[] GenerateQrCode(string uri)
        {
            using var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);

            // Remove 'using' for QRCode instance since it doesn't implement IDisposable
            var qrCode = new QRCode(qrCodeData);
            using var bitmap = qrCode.GetGraphic(20);
            using var stream = new MemoryStream();
            bitmap.Save(stream,ImageFormat.Png);
            return stream.ToArray();
        }

    }
}
