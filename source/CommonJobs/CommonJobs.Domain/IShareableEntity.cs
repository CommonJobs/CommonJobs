using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public interface IShareableEntity
    {
        string Id { get; set; }
        SharedLinkList SharedLinks { get; set; }
    }
}
