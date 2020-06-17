using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureSearchApp.Models
{
    public class Doctor
    {
        [System.ComponentModel.DataAnnotations.Key]
        [IsFilterable]
        public string Id { get; set; }
        [IsSearchable]
        public string FirstName { get; set; }
        [IsSearchable]
        public string LastName { get; set; }
        public string Email { get; set; }
        [IsFilterable, IsFacetable]
        public string Gender { get; set; }

        public Specialty[] Specialties { get; set; }
    }
}
