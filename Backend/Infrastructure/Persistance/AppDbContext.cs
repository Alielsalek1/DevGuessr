using Microsoft.EntityFrameworkCore;
using Domain.Models.UserRefreshTokens;
using Domain.Models.User;
using Domain.Models.UserDevice;
using LangdleModel = Domain.Models.Langdle.Langdle;
using Domain.Models.LogodleTarget;
using Domain.Models.Clusterdle;
using Domain.Models.DailyLangdle;
using Domain.Models.DailyLogodle;

namespace Infrastructure.Persistance;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<LangdleModel> LangdleTargets => Set<LangdleModel>();
    public DbSet<LogodleTarget> LogodleTargets => Set<LogodleTarget>();
    public DbSet<Clusterdle> ClusterdleTargets => Set<Clusterdle>();
    public DbSet<DailyLangdle> DailyLangdles => Set<DailyLangdle>();
    public DbSet<DailyLogodle> DailyLogodles => Set<DailyLogodle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}