using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackbonePOC1.Models
{
    public class ProductTypeConfiguration
    {
        public ProductType ProductType { get; set; }
        public string Description { get { return ProductType.ToString(); } }
        public ResultsViewType ResultsViewType { get; set; }
        public int PageSize { get; set; }
        public List<FacetConfiguration> FacetConfigurations { get; set; }
    }
}