using System;
using System.Linq;
using System.Linq.Expressions;
using XSpy.Database.Models.Tables;

namespace XSpy.Database
{
    public static class LinqExtensions
    {
        public static IQueryable<TSource> ApplyTableLimit<TSource>(this IQueryable<TSource> source,
            DataTableRequest request)
        {
            if (request.Length <= -1)
                return source;

            if (request.Length <= 0)
                request.Length = 10;
            else if (request.Length > 500)
                request.Length = 500;

            return source.Skip(request.Start).Take(request.Length);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperties()
                .FirstOrDefault(s => s.Name.Equals(ordering, StringComparison.InvariantCultureIgnoreCase));
            if (property == null)
                return source;
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderBy",
                new Type[] {type, property.PropertyType}, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }
        
        public static IQueryable<T> ThenBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperties()
                .FirstOrDefault(s => s.Name.Equals(ordering, StringComparison.InvariantCultureIgnoreCase));
            if (property == null)
                return source;
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "ThenBy",
                new Type[] {type, property.PropertyType}, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string ordering,
            params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperties()
                .FirstOrDefault(s => s.Name.Equals(ordering, StringComparison.InvariantCultureIgnoreCase));
            if (property == null)
                return source;
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "OrderByDescending",
                new Type[] {type, property.PropertyType}, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }
        public static IQueryable<T> ThenByDescending<T>(this IQueryable<T> source, string ordering,
            params object[] values)
        {
            var type = typeof(T);
            var property = type.GetProperties()
                .FirstOrDefault(s => s.Name.Equals(ordering, StringComparison.InvariantCultureIgnoreCase));
            if (property == null)
                return source;
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), "ThenByDescending",
                new Type[] {type, property.PropertyType}, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }


    }
}