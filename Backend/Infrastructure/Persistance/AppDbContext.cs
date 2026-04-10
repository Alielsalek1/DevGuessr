using Microsoft.EntityFrameworkCore;
using Domain.Models.UserRefreshTokens;
using Domain.Models.User;
using Domain.Models.UserDevice;
using Domain.Models.ProgrammingLanguage;
using Domain.Models.LogodleTarget;
using Domain.Models.TechnectionCategory;
using Domain.Models.DailyTechdle;
using Domain.Models.DailyLogodle;

namespace Infrastructure.Persistance;
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
    public DbSet<ProgrammingLanguage> ProgrammingLanguages => Set<ProgrammingLanguage>();
    public DbSet<LogodleTarget> LogodleTargets => Set<LogodleTarget>();
    public DbSet<TechnectionCategory> TechnectionCategories => Set<TechnectionCategory>();
    public DbSet<DailyTechdle> DailyTechdles => Set<DailyTechdle>();
    public DbSet<DailyLogodle> DailyLogodles => Set<DailyLogodle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}