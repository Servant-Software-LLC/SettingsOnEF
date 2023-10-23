using Data.Common.Extension;
using Data.Common.Utils.ConnectionString;
using SettingsOnEF.FileBasedProvider.Tests.Test_Classes;
using SettingsOnEF.Json;
using SettingsOnEF.Tests.Common;
using System.Data.JsonClient;
using System.Reflection;
using Xunit;

namespace SettingsOnEF.FileBasedProvider.Tests;


public class JsonProviderIntegrationTests
{
    [Fact]
    public void Get_WhenNoSettingExists_ReturnsNewSetting()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var settingsManager = new JsonSettingsManager(RandomSource(sandboxId));

        TestLogic.Get_WhenNoSettingExists_ReturnsNewSetting(settingsManager);
    }

    [Fact]
    public void Get_IgnoreOtherRowsInTable()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var settingsManager = new JsonSettingsManager(RandomSource(sandboxId));

        TestLogic.Get_IgnoreOtherRowsInTable(settingsManager);
    }

    [Fact]
    public void Update_WhenCalled_PersistsSettingToDb()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var settingsManager = new JsonSettingsManager(RandomSource(sandboxId));

        TestLogic.Update_WhenCalled_PersistsSettingToDb(settingsManager);
    }

    [Fact]
    public void Update_BetweenSettingManagerInstances_OnlyOneEntity()
    {
        //We want the same data source to be reused every time that a new JsonSettingsManager is created
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var dataSource = RandomSource(sandboxId);

        TestLogic.Update_BetweenSettingManagerInstances_OnlyOneEntity(() => new JsonSettingsManager(dataSource));
    }

    /// <summary>
    /// Verify that the property description is in the json file.  See <see cref="JsonDescriptionAttribute"/>
    /// </summary>
    [Fact]
    public void JsonFile_WritesPropertyDescription()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var dataSource = RandomSource(sandboxId);

        //Act 

        //The settings file is created during the construction of the JsonSettingsManager
        JsonSettingsManager jsonSettingsManager = new(dataSource);

        //Assert
        FileConnectionString fileConnectionString = new() { DataSource = dataSource.FullName };
        var jsonConnection = new JsonConnection(fileConnectionString);

        Assert.True(jsonConnection.GetPathType() == Data.Common.Enum.PathType.Directory);
        var pathJsonTable = jsonConnection.GetTablePath(nameof(SomeSettingWithDescription));
        Assert.True(File.Exists(pathJsonTable));

        var jsonTable = File.ReadAllText(pathJsonTable);
        Assert.Contains("The property to beat 'em all.", jsonTable);
    }

    [Fact]
    public void JsonFile_BetweenSettingManagerInstances_WritesPropertyDescription()
    {
        var sandboxId = $"{GetType().FullName}.{MethodBase.GetCurrentMethod()!.Name}";
        var dataSource = RandomSource(sandboxId);

        //Act 

        //The settings file is created during the construction of the JsonSettingsManager
        var guidString = Guid.NewGuid().ToString();
        using (JsonSettingsManager firstSettingsManager = new(dataSource))
        {
            var setting = firstSettingsManager.Get<SomeSettingWithDescription>();
            setting.SomeProperty = guidString;
            firstSettingsManager.Update(setting);
        }

        using (JsonSettingsManager secondSettingsManager = new(dataSource))
        {
            var setting = secondSettingsManager.Get<SomeSettingWithDescription>();
            Assert.Equal(guidString, setting.SomeProperty);

            var localGuidString = Guid.NewGuid().ToString();
            setting.SomeProperty = localGuidString;
            secondSettingsManager.Update(setting);
        }


        //Assert
        FileConnectionString fileConnectionString = new() { DataSource = dataSource.FullName };
        var jsonConnection = new JsonConnection(fileConnectionString);

        Assert.True(jsonConnection.GetPathType() == Data.Common.Enum.PathType.Directory);
        var pathJsonTable = jsonConnection.GetTablePath(nameof(SomeSettingWithDescription));
        Assert.True(File.Exists(pathJsonTable));

        var jsonTable = File.ReadAllText(pathJsonTable);
        Assert.Contains("The property to beat 'em all.", jsonTable);
    }

    /// <summary>
    /// Creates a data source with a random Guid
    /// </summary>
    /// <returns></returns>
    private static FileInfo RandomSource(string sandboxId) =>
        new FileInfo(Path.Combine(sandboxId, Guid.NewGuid().ToString()));
}