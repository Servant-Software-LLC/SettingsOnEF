using SettingsOnEF.Tests.Test_Classes;
using Xunit;

namespace SettingsOnEF.Tests.Common;

public static class TestLogic
{
    public static void Get_WhenNoSettingExists_ReturnsNewSetting(ISettingsManager settingsManager)
    {
        var result = settingsManager.Get<SomeSetting>();

        Assert.NotNull(result);
        Assert.Equal(1, settingsManager.DbContext.Set<SomeSetting>().Count());
    }

    public static void Get_IgnoreOtherRowsInTable(ISettingsManager settingsManager)
    {
        //Setup
        var setting = new SomeSetting { SomeProperty = "OriginalValue" };
        settingsManager.Update(setting);

        Assert.Equal(1, settingsManager.DbContext.Set<SomeSetting>().Count());

        //Add a second row to the table (outside of the SettingsOnEF library)
        var secondSetting = new SomeSetting { SomeProperty = "AnotherValue" };

        var entry = settingsManager.DbContext.Add(secondSetting);
        entry.Property(SettingsEntityAttribute.IdShadowProperty).CurrentValue = 2;

        settingsManager.DbContext.SaveChanges();

        Assert.Equal(2, settingsManager.DbContext.Set<SomeSetting>().Count());

        //Assert
        var result = settingsManager.Get<SomeSetting>();
        Assert.Equal(setting.SomeProperty, result.SomeProperty);
    }

    public static void Update_WhenCalled_PersistsSettingToDb(ISettingsManager settingsManager)
    {
        var setting = new SomeSetting { SomeProperty = "TestValue" };

        settingsManager.Update(setting);

        Assert.Equal(1, settingsManager.DbContext.Set<SomeSetting>().Count());

        var savedSetting = settingsManager.DbContext.Set<SomeSetting>().Single();
        Assert.Equal("TestValue", savedSetting.SomeProperty);
    }

    /// <summary>
    /// Tests that there is only one entity in the table between instances of the SettingsManager.
    /// </summary>
    /// <param name="createSettingsManager">Every time called, creates a <see cref="SettingsManager"/> using the same connection string.</param>
    public static void Update_BetweenSettingManagerInstances_OnlyOneEntity(Func<ISettingsManager> createSettingsManager)
    {
        var guidString = Guid.NewGuid().ToString();
        using (var firstSettingsManager = createSettingsManager())
        {
            var setting = firstSettingsManager.Get<SomeSetting>();
            Assert.True(string.IsNullOrEmpty(setting.SomeProperty));

            setting.SomeProperty = guidString;
            firstSettingsManager.Update(setting);

            Assert.Equal(1, firstSettingsManager.DbContext.Set<SomeSetting>().Count());

            Assert.Equal(guidString, setting.SomeProperty);
        }

        //There was a bug where a new entity was added to the table when restarting the application
        using (var secondSettingsManager = createSettingsManager())
        {
            var setting = secondSettingsManager.Get<SomeSetting>();
            Assert.Equal(guidString, setting.SomeProperty);

            var localGuidString = Guid.NewGuid().ToString();
            setting.SomeProperty = localGuidString;
            secondSettingsManager.Update(setting);

            Assert.Equal(1, secondSettingsManager.DbContext.Set<SomeSetting>().Count());

            Assert.Equal(localGuidString, setting.SomeProperty);
        }
    }
}
