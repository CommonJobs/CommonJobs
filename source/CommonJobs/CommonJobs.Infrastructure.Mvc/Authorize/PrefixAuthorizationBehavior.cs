using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Infrastructure.Mvc.Authorize
{
    public class PrefixAuthorizationBehavior : PrefixAuthorizationBehaviorBase
    {
        private string prefix;

        public override string Prefix 
        {
            get { return prefix; }
        }

        public PrefixAuthorizationBehavior(string prefix)
        {
            this.prefix = prefix;
        }
    }
}
