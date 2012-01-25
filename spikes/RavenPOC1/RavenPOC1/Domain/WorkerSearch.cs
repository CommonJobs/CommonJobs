using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RavenPOC1.Domain
{
    public class WorkerSearch
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Skills { get; set; }
    }
}
