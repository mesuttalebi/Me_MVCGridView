using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace mesoft.gridview.Models
{
    public static class ExpresssionBuilder
    {
        private static readonly MethodInfo containsMethod = typeof(string).GetMethod("Contains");
        private static readonly MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static readonly MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });


        public static Expression<Func<T,bool>> GetExpression<T>(IList<FilterObject> filters)
        {
            if (filters.Count == 0)
                return null;

            ParameterExpression param = Expression.Parameter(typeof(T), "t");
            Expression exp = null;

            if (filters.Count == 1)
                exp = GetExpression<T>(param, filters[0]);
            else if (filters.Count == 2)
                exp = GetExpression<T>(param, filters[0], filters[1]);
            else
            {
                while (filters.Count > 0)
                {
                    var f1 = filters[0];
                    var f2 = filters[1];

                    if (exp == null)
                        exp = GetExpression<T>(param, filters[0], filters[1]);
                    else
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0], filters[1]));

                    filters.Remove(f1);
                    filters.Remove(f2);

                    if (filters.Count == 1)
                    {
                        exp = Expression.AndAlso(exp, GetExpression<T>(param, filters[0]));
                        filters.RemoveAt(0);
                    }
                }
            }

            return Expression.Lambda<Func<T, bool>>(exp, param);
        }

        private static Expression GetExpression<T>(ParameterExpression param, FilterObject filter)
        {
            MemberExpression member = Expression.Property(param, filter.Column);
            //ConstantExpression constant = Expression.Constant(filter.Value);

            // NEW LOGIC to handle nullable Decimal and DateTime values
            UnaryExpression constant = null;
            if (member.Type == typeof(Decimal?))
            {
                constant = Expression.Convert(Expression.Constant(Decimal.Parse(filter.Value)) , member.Type);
            }
            else if (member.Type == typeof(DateTime?))
            {
                constant = Expression.Convert(Expression.Constant(DateTime.Parse(filter.Value)), member.Type);
            }
            else
            {
                constant = Expression.Convert(Expression.Constant(filter.Value), member.Type);
            }


            switch (filter.Operator)
            {
               case FilterOperator.Equals:
                    return Expression.Equal(member, constant);

                case FilterOperator.GreaterThan:
                    return Expression.GreaterThan(member, constant);

                case FilterOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(member, constant);

                case FilterOperator.LessThan:
                    return Expression.LessThan(member, constant);

                case FilterOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(member, constant);

                case FilterOperator.Contains:
                    return Expression.Call(member, containsMethod, constant);

                case FilterOperator.StartsWith:
                    return Expression.Call(member, startsWithMethod, constant);

                case FilterOperator.EndsWith:
                    return Expression.Call(member, endsWithMethod, constant);

                case FilterOperator.NotEqual:
                    return Expression.Negate(Expression.Equal(member, constant));
            }

            return null;
        }

        private static BinaryExpression GetExpression<T> (ParameterExpression param, FilterObject filter1, FilterObject filter2)
        {
            Expression bin1 = GetExpression<T>(param, filter1);
            Expression bin2 = GetExpression<T>(param, filter2);

            return Expression.AndAlso(bin1, bin2);
        }
    }
}