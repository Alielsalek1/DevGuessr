namespace Application.DTOs.Auth.InternalAuth;

public record NewDeviceOtpPayload(Guid UserId, Guid DeviceId);
