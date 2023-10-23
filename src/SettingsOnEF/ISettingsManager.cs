namespace SettingsOnEF;

public interface ISettingsManager : IDisposable
{
    SettingsDbContext DbContext { get; }
    TSettingsEntity Get<TSettingsEntity>() 
        where TSettingsEntity : class, new();
    void Update<TSettingsEntity>(TSettingsEntity settings)
        where TSettingsEntity : class, new();
}
