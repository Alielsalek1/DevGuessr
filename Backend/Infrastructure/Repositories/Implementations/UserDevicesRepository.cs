namespace Infrastructure.Repositories.Implementations;

using Domain.Models.UserDevice;
using Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Application.Repositories.Interfaces;

public class UserDevicesRepository(AppDbContext dbContext) : IUserDevicesRepository
{
    private readonly AppDbContext _dbContext = dbContext;
    public async Task AddUserDeviceAsync(UserDevice userDevice, CancellationToken cancellationToken)
    {
        await _dbContext.UserDevices.AddAsync(userDevice, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> IsDeviceIdPresentForUserId(Guid deviceId, Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserDevices.AsNoTracking()
            .AnyAsync(ud => ud.UserId == userId && ud.DeviceId == deviceId, cancellationToken);
    }
}