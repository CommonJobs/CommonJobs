using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class SharedLinkList : List<SharedLink>
    {
        public SharedLink GetBySharedCode(string sharedCode)
        {
            return this.FirstOrDefault(x => x.SharedCode == sharedCode);
        }
    }
}
