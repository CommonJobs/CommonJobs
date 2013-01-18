using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain.MyMenu
{
    public struct Place : IKeyText
    {
        public string Key { get; set; }
        public string Text { get; set; }
    }
}
