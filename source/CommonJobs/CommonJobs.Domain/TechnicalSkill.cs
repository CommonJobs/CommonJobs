using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

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
                if (Name == null)
                    return null;
                var result = Name.GenerateSlug().Replace("_", ""); 
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
