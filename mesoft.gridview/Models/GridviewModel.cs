/*
    megridview v0.3.1
    Developed By Mesut Talebi (mesut.talebi@yahoo.com)
    Open Source And no licence :) free to use 
*/


using System;
using System.Collections.Generic;

namespace mesoft.gridview.Models
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
            get { return (int)Math.Ceiling((decimal)TotalItems / (ItemsPerPage != 0 ? ItemsPerPage : 1)); }
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

    public class Extensions
    {
        public static string GetWhereClause(FilterObject filterObj, Type valueType)
        {           
            string whereClause = "true";
            if (valueType != typeof (DateTime))
            {
                switch (filterObj.Operator)
                {
                    case FilterOperator.Contains:
                        if (valueType == typeof (string))
                            whereClause += string.Format(" {0} {1}.Contains(\"{2}\")", filterObj.Conjunction,
                                filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.GreaterThan:
                        if (valueType != typeof (string))
                            whereClause += string.Format(" {0} {1} > {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.GreaterThanOrEqual:
                        if (valueType != typeof (string))
                            whereClause += string.Format(" {0} {1} >= {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.LessThan:
                        if (valueType != typeof (string))
                            whereClause += string.Format(" {0} {1} < {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.LessThanOrEqual:
                        if (valueType != typeof (string))
                            whereClause += string.Format(" {0} {1} <= {2}", filterObj.Conjunction, filterObj.Column,
                                filterObj.Value);
                        break;
                    case FilterOperator.StartsWith:
                        if (valueType != typeof (string))
                            whereClause += string.Format(" {0} {1}.StartsWith(\"{2}\")", filterObj.Conjunction,
                                filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.EndsWith:
                        if (valueType != typeof (string))
                            whereClause += string.Format(" {0} {1}.EndsWith(\"{2}\")", filterObj.Conjunction,
                                filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.Equals:

                        whereClause +=
                            string.Format(valueType == typeof (string) ? " {0} {1} == \"{2}\"" : " {0} {1} == {2}",
                                filterObj.Conjunction, filterObj.Column, filterObj.Value);
                        break;
                    case FilterOperator.NotEqual:

                        whereClause +=
                            string.Format(valueType == typeof (string) ? " {0} {1} != \"{2}\"" : " {0} {1} != {2}",
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