using Domain.Exceptions;

namespace Domain.Constraints.userDevice;

public static class UserDeviceGuard
{

    public static void ValidateUserId(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new DomainException("User ID cannot be empty.");
    }

    public static void ValidateDeviceId(Guid deviceId)
    {
        if (deviceId == Guid.Empty)
            throw new DomainException("Device ID cannot be empty.");
    }
}