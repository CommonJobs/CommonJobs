using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class EventType
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public string Slug { get { return Text.GenerateSlug(); } }

        protected EventType()
        {
            //RavenDB usage
        }

        public EventType(string text, string color)
        {
            Text = text;
            Color = color;
        }

        public bool IsMatch(string value)
        {
            return Match(Text, value);
        }

        public static bool Match(string value1, string value2)
        {
            return value1.GenerateSlug() == value2.GenerateSlug();
        }

        public override bool Equals(object obj)
        {
            var other = obj as EventType;
            return (other != null && other.Slug == this.Slug) || base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Slug != null ? Slug.GetHashCode() : base.GetHashCode();
        }
    }
}
