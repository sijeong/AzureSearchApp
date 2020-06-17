using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearchApp.Models
{
    public class SearchData
    {
        public SearchData() { }
        public SearchData(DocumentSearchResult<Hotel> d) {
            this.ResultList = d;
        }
        //public SearchData() { this.ResultList = new DocumentSearchResult<Hotel>(new List<SearchResult<Hotel>(), null, null, )}
        public string SearchText { get; set; }
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
        public int LeftMostPage { get; set; }
        public int PageRange { get; set; }
        public string Paging { get; set; }

        public string categoryFilter { get; set; }
        public string amenityFilter { get; set; }
        //[NotNull]
        public DocumentSearchResult<Hotel> ResultList { get; set; }

        //private DocumentSearchResult<Hotel> resultList;
    }

    public static class GlobalVariables
    {
        public static int ResultsPerPage
        {
            get
            {
                return 3;
            }
        }

        public static int MaxPageRange
        {
            get
            {
                return 5;
            }
        }

        public static int PageRangeDelta
        {
            get
            {
                return 2;
            }
        }
    }
}
