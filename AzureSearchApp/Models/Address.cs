using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearchApp.Models
{
    public partial class Address
    {
        [IsSearchable]
        public string StreeAddress { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string City { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string StateProvince { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string PostalCode { get; set; }
        [IsSearchable, IsFilterable, IsSortable, IsFacetable]
        public string Country { get; set; }
    }
}
