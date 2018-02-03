/*
    megridview v0.4.0
    Developed By Mesut Talebi (mesut.talebi@yahoo.com)
    Open Source And no licence :) free to use 
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MT.GridView.Models
{
    public class PagingInfo
    {
        public List<int> PageOptions { get; set; }

        public bool ShowPageOptions { get; set; }

        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ( ItemsPerPage != 0 ? ItemsPerPage : 1 )); }
        }

        public SortObject Sort { get; set; }

        public IList<FilterObject> Filters { get; set; }

        public string SearchTerm { get; set; }
    }

    public class SortObject
    {
        public String SortColumn { get; set; }

        public SortDirection Direction { get; set; }
    }

    public class FilterObject
    {
        public string Column { get; set; }

        public string Value { get; set; }

        public FilterOperator Operator { get; set; }

        public FilterConjunction Conjunction { get; set; }
    }


    /********* ENUMS *************/
    public enum SortDirection
    {
        NotSet,
        Ascending,
        Descending
    }

    public enum FilterOperator
    {
        Contains,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        StartsWith,
        EndsWith,
        Equals,
        NotEqual
    }

    public enum FilterConjunction
    {
        And,
        Or
    }

    public static class Extensions
    {
        private static LambdaExpression GetExpression<T>(string propertyName, string methodName, out Type type)
        {
            string[] props = propertyName.Split('.');
            type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;

            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }

            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            
            return Expression.Lambda(delegateType, expr, arg);
        }

        private static IOrderedEnumerable<T> ApplyOrder<T>(
            IEnumerable<T> source,
            string propertyName, 
            string methodName)
        {
            Type type = typeof(T);
            LambdaExpression lambda = GetExpression<T>(propertyName, methodName, out type);

            object result = typeof(Queryable).GetMethods().Single(
                   method => method.Name == methodName
                           && method.IsGenericMethodDefinition
                           && method.GetGenericArguments().Length == 2
                           && method.GetParameters().Length == 2)
                   .MakeGenericMethod(typeof(T), type)
                   .Invoke(null, new object[] { source, lambda });

            return (IOrderedEnumerable<T>)result;
        }


        private static IOrderedQueryable<T> ApplyOrder<T>(
            IQueryable<T> source, 
            string propertyName, string methodName)
        {
            Type type = typeof(T);
            LambdaExpression lambda = GetExpression<T>(propertyName, methodName, out type);

            object result = typeof(Queryable).GetMethods().Single(
                   method => method.Name == methodName
                           && method.IsGenericMethodDefinition
                           && method.GetGenericArguments().Length == 2
                           && method.GetParameters().Length == 2)
                   .MakeGenericMethod(typeof(T), type)
                   .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }
       
        // IQueryable Sort Extention
        public static IOrderedQueryable<TSource> Sort<TSource>(this IQueryable<TSource> source, SortObject sortObject)
        {
            if (sortObject != null)
            {
                switch (sortObject.Direction)
                {
                    case SortDirection.Ascending:
                        return ApplyOrder<TSource>(source, sortObject.SortColumn, "OrderBy");
                    case SortDirection.Descending:
                        return ApplyOrder<TSource>(source, sortObject.SortColumn, "OrderByDescending");
                    default:
                        break;
                }
            }

            return source as IOrderedQueryable<TSource>;
        }

        public static IOrderedEnumerable<TSource> Sort<TSource>(this IEnumerable<TSource> source, SortObject sortObject)
        {
            if (sortObject != null)
            {
                switch (sortObject.Direction)
                {
                    case SortDirection.Ascending:
                        return ApplyOrder<TSource>(source, sortObject.SortColumn, "OrderBy");
                    case SortDirection.Descending:
                        return ApplyOrder<TSource>(source, sortObject.SortColumn, "OrderByDescending");
                    default:
                        break;
                }
            }

            return source as IOrderedEnumerable<TSource>;
        }

        public static string GetWhereClause(FilterObject filterObj, Type valueType)
        {
            string whereClause = "true";
            if (valueType != typeof(DateTime))
            {
                switch (filterObj.Operator)
                {
                    case FilterOperator.Contains:
                        if (valueType == typeof(string))
                            whereClause += string.Format(" {0} {1}.Contains(\"{2}\")", filterObj.Conjunction,
                                filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.GreaterThan:
                        if (valueType != typeof(string))
                            whereClause += string.Format(" {0} {1} > {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.GreaterThanOrEqual:
                        if (valueType != typeof(string))
                            whereClause += string.Format(" {0} {1} >= {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.LessThan:
                        if (valueType != typeof(string))
                            whereClause += string.Format(" {0} {1} < {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.LessThanOrEqual:
                        if (valueType != typeof(string))
                            whereClause += string.Format(" {0} {1} <= {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.StartsWith:
                        if (valueType != typeof(string))
                            whereClause += string.Format(" {0} {1}.StartsWith(\"{2}\")", filterObj.Conjunction,
                                filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.EndsWith:
                        if (valueType != typeof(string))
                            whereClause += string.Format(" {0} {1}.EndsWith(\"{2}\")", filterObj.Conjunction,
                                filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.Equals:

                        whereClause +=
                            string.Format(valueType == typeof(string) ? " {0} {1} == \"{2}\"" : " {0} {1} == {2}",
                                filterObj.Conjunction, filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.NotEqual:

                        whereClause +=
                            string.Format(valueType == typeof(string) ? " {0} {1} != \"{2}\"" : " {0} {1} != {2}",
                                filterObj.Conjunction, filterObj.Column, filterObj.Value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                DateTime dt;
                DateTime.TryParse(filterObj.Value, out dt);

                switch (filterObj.Operator)
                {
                    case FilterOperator.Contains:
                        break;
                    case FilterOperator.GreaterThan:

                        whereClause += string.Format(" {0} {1} > DateTime(\"{2}\")", filterObj.Conjunction, filterObj.Column, dt);
                        break;
                    case FilterOperator.GreaterThanOrEqual:

                        whereClause += string.Format(" {0} {1} >= DateTime(\"{2}\")", filterObj.Conjunction, filterObj.Column, dt);
                        break;
                    case FilterOperator.LessThan:

                        whereClause += string.Format(" {0} {1} <  DateTime(\"{2}\")", filterObj.Conjunction, filterObj.Column, dt);
                        break;
                    case FilterOperator.LessThanOrEqual:
                        whereClause += string.Format(" {0} {1} <=  DateTime(\"{2}\")", filterObj.Conjunction, filterObj.Column, dt);
                        break;
                    case FilterOperator.StartsWith:
                        break;
                    case FilterOperator.EndsWith:
                        break;
                    case FilterOperator.Equals:
                        whereClause += string.Format(" {0} {1} ==  DateTime(\"{2}\")", filterObj.Conjunction, filterObj.Column, dt);
                        break;
                    case FilterOperator.NotEqual:
                        whereClause += string.Format(" {0} {1} !=  DateTime(\"{2}\")", filterObj.Conjunction, filterObj.Column, dt);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return whereClause;
        }
    }
}