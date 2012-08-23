using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Domain
{
    public class AttachmentSlot
    {
        private const string IdPattern = "AttachmentSlots/{0}/{1}";

        private AttachmentSlot()
        {
            //RavenDB usage
        }

        public AttachmentSlot(Type relatedEntityType, string name)
        {
            RelatedEntityTypeName = relatedEntityType.Name;
            Name = name;
            Description = name;
            Id = GenerateId(relatedEntityType, name);
        }
        
        public string Id { get; private set; }

        public string RelatedEntityTypeName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public AttachmentSlotNecessity Necessity { get; set; }

        public static string GenerateId(Type relatedEntityType, string name)
        {
            return string.Format(IdPattern, relatedEntityType.Name, name);
        }
    }
}
