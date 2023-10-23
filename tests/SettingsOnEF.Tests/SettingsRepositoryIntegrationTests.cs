using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SettingsOnEF.Tests.Common;
using Xunit;

namespace SettingsOnEF.Tests;

public class SettingsRepositoryIntegrationTests : IDisposable
{
    private readonly SettingsManager settingsManager;

    public SettingsRepositoryIntegrationTests()
    {
        settingsManager = new SettingsManager(contextBuilder => contextBuilder.UseSqlite($"Data Source={RandomSource().FullName};Mode=Memory;Cache=Shared"));
    }

    [Fact]
    public void Get_WhenNoSettingExists_ReturnsNewSetting() => TestLogic.Get_WhenNoSettingExists_ReturnsNewSetting(settingsManager);

    [Fact]
    public void Get_IgnoreOtherRowsInTable() => TestLogic.Get_IgnoreOtherRowsInTable(settingsManager);

    [Fact]
    public void Update_WhenCalled_PersistsSettingToDb() => TestLogic.Update_WhenCalled_PersistsSettingToDb(settingsManager);

    [Fact]
    public void Update_BetweenSettingManagerInstances_OnlyOneEntity()
    {
        //We want the same data source to be reused every time that a new JsonSettingsManager is created
        var dataSource = RandomSource();

        using (var connection = new SqliteConnection("Data Source={dataSource};Mode=Memory;Cache=Shared"))
        {
            connection.Open();

            TestLogic.Update_BetweenSettingManagerInstances_OnlyOneEntity(() =>
                new SettingsManager(contextBuilder => contextBuilder.UseSqlite(connection)));

            connection.Close();
        }
    }

    public void Dispose()
    {
        settingsManager?.Dispose();
    }

    /// <summary>
    /// Creates a data source with a random Guid
    /// </summary>
    /// <returns></returns>
    private static FileInfo RandomSource() =>
        new FileInfo(Guid.NewGuid().ToString());
}
