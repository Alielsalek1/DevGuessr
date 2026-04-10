namespace Application.Repositories.Interfaces;

using Domain.Models.UserDevice;

public interface IUserDevicesRepository
{
    Task AddUserDeviceAsync(UserDevice userDevice, CancellationToken cancellationToken);
    Task<bool> IsDeviceIdPresentForUserId(Guid deviceId, Guid userId, CancellationToken cancellationToken);
}