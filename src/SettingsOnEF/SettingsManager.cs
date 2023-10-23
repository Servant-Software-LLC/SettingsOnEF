using Microsoft.EntityFrameworkCore;
using SettingsOnEF.Extensions;

namespace SettingsOnEF;

public class SettingsManager : ISettingsManager, IDisposable
{
    public SettingsManager(Action<DbContextOptionsBuilder> configureContextForDatabase)
    {
        var dbContextOptionsBuilder = new DbContextOptionsBuilder<SettingsDbContext>();
        configureContextForDatabase(dbContextOptionsBuilder);
        var options = dbContextOptionsBuilder.Options;

        DbContext = new SettingsDbContext(options);
        DbContext.Database.OpenConnection();
        DbContext.Database.EnsureCreated();

        EnsureAllEntitiesHaveARow();
    }

    /// <summary>
    /// Adds a row to each of the setting entity types.
    /// </summary>
    private void EnsureAllEntitiesHaveARow()
    {
        var model = DbContext.Model;
        foreach (var entityType in model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            var entityCount = DbContext.GetEntityCount(clrType);
            if (entityCount == 0)
            {
                var entity = Activator.CreateInstance(clrType);
                var entry = DbContext.Add(entity!);

                //Set the shadow property that we added in SettingsDbContext.OnModelCreating()
                entry.Property(SettingsEntityAttribute.IdShadowProperty).CurrentValue = 1;
                DbContext.SaveChanges();
            }
        }

    }

    public SettingsDbContext DbContext { get; }

    public TSettingsEntity Get<TSettingsEntity>() where TSettingsEntity : class, new()
    {
        var repository = new SettingsRepository<TSettingsEntity>(DbContext);
        return repository.Get();
    }

    public void Update<TSettingsEntity>(TSettingsEntity settings) where TSettingsEntity : class, new()
    {
        var repository = new SettingsRepository<TSettingsEntity>(DbContext);
        repository.Update(settings);
    }

    public void Dispose()
    {
        DbContext.Database.CloseConnection();
        DbContext?.Dispose();
    }

}
