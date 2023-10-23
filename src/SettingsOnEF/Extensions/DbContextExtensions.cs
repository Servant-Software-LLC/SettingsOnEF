using Microsoft.EntityFrameworkCore;

namespace SettingsOnEF.Extensions;

public static class DbContextExtensions
{
    public static int GetEntityCount(this DbContext context, Type entityType)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (entityType == null) throw new ArgumentNullException(nameof(entityType));

        // Get the DbSet for the given entity type using DbContext.Set<TEntity>()
        var setMethod = typeof(DbContext).GetMethods()
            .Where(m => m.IsGenericMethod && m.Name == nameof(DbContext.Set) && m.IsPublic && m.DeclaringType == typeof(DbContext))
            .FirstOrDefault();

        if (setMethod == null)
        {
            throw new InvalidOperationException($"Unable to find the {nameof(DbContext.Set)} method on DbContext.");
        }

        var genericSetMethod = setMethod!.MakeGenericMethod(entityType);
        var dbSet = genericSetMethod.Invoke(context, null);

        if (dbSet == null)
            return 0;

        // Call the Count() method on the DbSet
        // Get the Count method from the Queryable class
        var countMethod = typeof(Queryable).GetMethods()
            .Where(m => m.Name == nameof(Queryable.Count) && m.IsStatic)
            .Select(m => new { Method = m, Parameters = m.GetParameters() })
            .Where(x => x.Parameters.Length == 1 && x.Parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .Select(x => x.Method)
            .FirstOrDefault();

        if (countMethod == null)
        {
            throw new InvalidOperationException("Unable to find the Count method on Queryable.");
        }

        var genericCountMethod = countMethod.MakeGenericMethod(entityType);

        var count = genericCountMethod.Invoke(null, new object[] { dbSet });
        if (count == null) 
            return -1;

        return (int)count;
    }
}
