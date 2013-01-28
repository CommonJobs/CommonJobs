using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public enum TechnicalSkillLevel
    {
        Unknown = 0,
        Basic = 1,
        Intermediate = 2,
        Advanced = 3
    }

    public static class TechnicalSkillLevelExtensions
    {
        public static string[] GetLevelNames()
        {
            return Enum.GetNames(typeof(TechnicalSkillLevel));
        }

        public static IEnumerable<TechnicalSkillLevel> GetValues()
        {
            return Enum.GetValues(typeof(TechnicalSkillLevel))
                .Cast<TechnicalSkillLevel>();
        }
    }
}
