using Microsoft.EntityFrameworkCore;
using Moq;
using SettingsOnEF.Tests.Test_Classes;
using Xunit;

namespace SettingsOnEF.Tests;

public class SettingsRepositoryTests
{

    [Fact]
    public void Get_WhenNoSettingExists_ReturnsNewSetting()
    {
        // Setup a mock of DbSet<SomeSetting> to simulate an empty database.
        var(repository, dbSetMock, dbContextMock) = SetupMocks(new());

        // 2. Test the behavior of the Get() method on your repository.
        var result = repository.Get();

        // 3. Assert that the method correctly initializes a new setting if none exists.
        Assert.NotNull(result);
        dbSetMock.Verify(m => m.Add(It.IsAny<SomeSetting>()), Times.Once);
        dbContextMock.Verify(m => m.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Get_WhenSettingExists_ReturnsExistingSetting()
    {
        var expectedSetting = new SomeSetting { SomeProperty = "ExistingValue" };
        var (repository, dbSetMock, dbContextMock) = SetupMocks(new() { expectedSetting });

        var result = repository.Get();

        Assert.Equal(expectedSetting, result);
        dbSetMock.Verify(m => m.Add(It.IsAny<SomeSetting>()), Times.Never);
        dbContextMock.Verify(m => m.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Update_WhenCalled_SavesSettingToDb()
    {
        var setting = new SomeSetting { SomeProperty = "TestValue" };
        var (repository, dbSetMock, dbContextMock) = SetupMocks(new() { setting });

        repository.Update(setting);

        dbContextMock.Verify(m => m.SaveChanges(), Times.AtLeastOnce);
    }

    private static (SettingsRepository<SomeSetting> Repository, Mock<DbSet<SomeSetting>> Set, Mock<SettingsDbContext> DbContext) SetupMocks(List<SomeSetting> listSettings)
    {
        var data = listSettings.AsQueryable();
        Mock<DbSet<SomeSetting>> mockSet = new();
        mockSet.As<IQueryable<SomeSetting>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<SomeSetting>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<SomeSetting>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<SomeSetting>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        // Assuming dbContextMock has been set up to return mockSet.Object for its SomeSettings property.
        Mock<SettingsDbContext> dbContextMock = new(new DbContextOptions<SettingsDbContext>());
        dbContextMock.Setup(db => db.Set<SomeSetting>()).Returns(mockSet.Object);

        return (new SettingsRepository<SomeSetting>(dbContextMock.Object), mockSet, dbContextMock);
    }


}
