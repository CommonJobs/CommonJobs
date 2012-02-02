using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackbonePOC1.Models
{
    public class CategoryConfiguration
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public ViewType ViewType { get; set; }
        public bool CollapsedAsDefault { get; set; }
        public int ItemsToShow { get; set; }
    }
}