using FluentEmail.Core;

namespace Application.Services.Implementations.Misc;

public class PasswordResetEmailSender(IFluentEmail fluentEmail) : EmailSenderBase(fluentEmail)
{
    protected override string GetSubject() => "Reset Your Password - Recovery Code";

    protected override string GetBody(string token) => $"""
            <div style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 12px; background-color: #ffffff;">
                <div style="background-color: #4f46e5; color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0;">
                    <h1 style="margin: 0; font-size: 24px; font-weight: 700;">Reset Your Password</h1>
                </div>
                <div style="padding: 40px; line-height: 1.8; color: #1e293b;">
                    <p style="font-size: 16px; margin-bottom: 24px;">Hello,</p>
                    <p style="font-size: 16px; margin-bottom: 24px;">We received a request to reset your password. To secure your realm, please use the following reset code:</p>
                    <div style="display: block; width: fit-content; margin: 32px auto; padding: 20px 40px; background-color: #f8fafc; border: 2px dashed #4f46e5; border-radius: 12px; font-size: 32px; font-weight: 800; color: #4f46e5; letter-spacing: 8px; text-align: center; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);">
                        {token}
                    </div>
                    <p style="font-size: 14px; color: #64748b; margin-top: 32px;">This code will expire in <strong>10 minutes</strong>. If you did not request this password reset, please ignore this email and your password will remain unchanged.</p>
                </div>
                <div style="font-size: 12px; color: #94a3b8; text-align: center; padding: 20px; border-top: 1px solid #f1f5f9;">
                    &copy; 2026 The Architect's Forge. Powered by .NET 9 & Distributed Wisdom.
                </div>
            </div>
            """;
}