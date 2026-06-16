namespace Application.Interfaces.Services;

public interface IQrCodeService
{
    string GenerateQrCode(string data);
}
