using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearchApp.Models
{
    public class Specialty
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string Id { get; set; }
        [IsSearchable]
        public string Name { get; set; }
        //[IsSearchable]
        //public string Description { get; set; }
    }
}
