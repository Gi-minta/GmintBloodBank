using Application.Interfaces.Services;

namespace Infrastructure.Services;

public sealed class QrCodeService : IQrCodeService
{
    public string GenerateQrCode(string data)
    {
        // TODO: Implement actual QR code generation
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data));
    }
}
