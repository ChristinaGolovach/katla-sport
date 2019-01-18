using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Extensions = System.Data.Entity.QueryableExtensions;

namespace KatlaSport.DataAccess
{
    public static class QueriableExtensions
    {
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
            where T : class
        {
            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            return Extensions.ToListAsync(source);
        }

        public static Task<T[]> ToArrayAsync<T>(this IQueryable<T> source)
            where T : class
        {
            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            return Extensions.ToArrayAsync(source);
        }

        public static Task<int> CountAsync<T>(this IQueryable<T> source)
            where T : class
        {
            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            return Extensions.CountAsync(source);
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate)
            where T : class
        {
            if (predicate == null)
            {
                throw new ArgumentNullException($"The {nameof(predicate)} can not be null.");
            }

            if (source is EntitySet<T>)
            {
                source = (source as EntitySet<T>).DbSet;
            }

            T result = await Extensions.FirstOrDefaultAsync(source, predicate).ConfigureAwait(continueOnCapturedContext: false);

            return result;
        }
    }
}
