using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackbonePOC1.Models
{
    public class ProductTypeConfiguration
    {
        public ProductType ProductType { get; set; }
        public ResultsViewType ResultsViewType { get; set; }
        public int PageSize { get; set; }
        public List<FacetConfiguration> FacetConfigurations { get; set; }
    }
}