using System.Security.Cryptography;

namespace Application.Utils;

public static class OtpGenerator
{
    public static string GenerateOtp()
        => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
}