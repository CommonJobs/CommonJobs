using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public interface IShareableResource
    {
        SharedLinkList SharedLinks { get; set; }
    }
}
