using Microsoft.AspNetCore.Identity;
using QRCoder.Core;
using System.Threading.Tasks;
using System;
using MahantInv.Infrastructure.Identity;

namespace MahantInv.Web.Service
{
    public class MfaService
    {
        private readonly UserManager<MIIdentityUser> _userManager;

        public MfaService(UserManager<MIIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> GenerateQrCodeAsync(MIIdentityUser user)
        {
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var qrCodeUri = GenerateQrCodeUri(user.Email, authenticatorKey);
            return GenerateQrCodeImage(qrCodeUri);
        }

        private string GenerateQrCodeUri(string email, string key)
        {
            return $"otpauth://totp/MyApp:{email}?secret={key}&issuer=MyApp&digits=6";
        }

        private string GenerateQrCodeImage(string qrCodeUri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
        }
    }
}
