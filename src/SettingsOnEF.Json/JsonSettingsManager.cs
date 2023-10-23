using Data.Common.Utils.ConnectionString;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace SettingsOnEF.Json;

public class JsonSettingsManager : ISettingsManager, IDisposable
{
    private readonly SettingsManager settingsManager;

    private static string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
    private const string settingsFolderName = "Settings";

    public JsonSettingsManager(string productName, LogLevel? logLevel = null)
        :this(new FileInfo(Path.Combine(basePath, productName, settingsFolderName)), logLevel)
    {
        
    }

    public JsonSettingsManager(string companyName, string productName, LogLevel? logLevel = null)
        : this(new FileInfo(Path.Combine(basePath, companyName, productName, settingsFolderName)), logLevel)
    {

    }

    public JsonSettingsManager(FileInfo jsonSettingsPath, LogLevel? logLevel = null)
    {
        FileConnectionString fileConnectionString = new()
        {
            DataSource = jsonSettingsPath.FullName,
            CreateIfNotExist = true,
            LogLevel = logLevel,
            Formatted = true
        };

        var jsonConnection = new JsonConnectionEx(fileConnectionString, SettingsDbContext.SettingsTypes.Value);

        settingsManager = new SettingsManager(contextBuilder => contextBuilder.UseJson(jsonConnection));
    }

    public SettingsDbContext DbContext => settingsManager.DbContext;

    public TSettingsEntity Get<TSettingsEntity>() where TSettingsEntity : class, new() =>
        settingsManager.Get<TSettingsEntity>();

    public void Update<TSettingsEntity>(TSettingsEntity settings) where TSettingsEntity : class, new() =>
        settingsManager.Update<TSettingsEntity>(settings);

    public void Dispose() => settingsManager.Dispose();

}
