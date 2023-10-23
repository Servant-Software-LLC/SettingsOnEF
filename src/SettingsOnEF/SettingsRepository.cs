namespace SettingsOnEF;

public class SettingsRepository<TSettingsEntity> : ISettingsRepository 
    where TSettingsEntity : class, new()
{
    private readonly SettingsDbContext dbContext;

    public SettingsRepository(SettingsDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public TSettingsEntity Get()
    {
        var setting = dbContext.Set<TSettingsEntity>().FirstOrDefault();

        if (setting == null)
        {
            setting = new TSettingsEntity();
            AddEntity(setting);
        }

        return setting;
    }

    public void Update(TSettingsEntity settings)
    {
        var originalEntity = dbContext.Set<TSettingsEntity>().FirstOrDefault();
        if (originalEntity != null)
        {
            dbContext.Set<TSettingsEntity>().Remove(originalEntity);
            dbContext.SaveChanges();
        }

        AddEntity(settings);
    }

    private void AddEntity(TSettingsEntity settings)
    {
        var entry = dbContext.Set<TSettingsEntity>().Add(settings);

        //Set the shadow property
        if (entry != null)  //Entry is only ever null in case of a mock in unit tests.
            entry.Property(SettingsEntityAttribute.IdShadowProperty).CurrentValue = 1;

        dbContext.SaveChanges();
    }

}

