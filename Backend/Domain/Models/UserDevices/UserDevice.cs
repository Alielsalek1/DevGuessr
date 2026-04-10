using Domain.Constraints.userDevice;

namespace Domain.Models.UserDevice;

public class UserDevice
{
    public Guid UserId { get; private set; }
    public Guid DeviceId { get; private set; }

    public UserDevice(Guid userId, Guid deviceId)
    {
        UserDeviceGuard.ValidateUserId(userId);
        UserDeviceGuard.ValidateDeviceId(deviceId);

        UserId = userId;
        DeviceId = deviceId;
    }
}