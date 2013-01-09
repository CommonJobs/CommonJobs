using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class TechnicalSkill
    {
        public string Name { get; set; }
        public TechnicalSkillLevel Level { get; set; }
        
        public int Weight 
        {
            get { return (int)Level; }
        }

        public string Searcheable
        {
            get
            {
                var result = Name.Replace(" ", "").Replace("_", ""); //TODO: Slug
                if (Level > 0) {
                    for (var l = 1; l <= (int)Level; l++) {
                        result += "_" + l;
                    }
                }
                return result;
            }
        }
    }
}
