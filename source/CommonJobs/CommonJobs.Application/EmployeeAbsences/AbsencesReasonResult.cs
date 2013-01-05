using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CommonJobs.Domain;
using CommonJobs.Utilities;

namespace CommonJobs.Application.EmployeeAbsences
{
    public class AbsencesReasonResult
    {
        public string Slug { get { return Text.GenerateSlug(); } }
        public string Text { get; set; }
        public string Color { get; set; }
        public bool Predefined { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as AbsencesReasonResult;
            return (other != null && other.Slug == this.Slug) || base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Slug != null ? Slug.GetHashCode() : base.GetHashCode();
        }
    }
}