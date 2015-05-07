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
}