using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackbonePOC1.Models
{
    public class FacetConfiguration
    {
        public string FacetName { get; set; }
        public bool Collapsed { get; set; }
        public int FiltersInitialyShowed { get; set; }
        public FacetFiltersOrderType OrderType { get; set; }
        public FacetFiltersOrderDirection OrderDirection { get; set; }
        public List<string> TopFilters { get; set; }
    }
}