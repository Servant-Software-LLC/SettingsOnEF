using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SettingsOnEF;

public class SettingsDbContext : DbContext
{
    public static Lazy<List<Type>> SettingsTypes = new Lazy<List<Type>>(() => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(GetTypesSafe)
            .Where(p => Attribute.IsDefined(p, typeof(SettingsEntityAttribute))).ToList());

    public SettingsDbContext(DbContextOptions options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var type in SettingsTypes.Value)
        {
            var entityType = modelBuilder.Entity(type);
            entityType.Property<int>(SettingsEntityAttribute.IdShadowProperty);
            entityType.HasKey(SettingsEntityAttribute.IdShadowProperty);
        }
    }

    private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }
}