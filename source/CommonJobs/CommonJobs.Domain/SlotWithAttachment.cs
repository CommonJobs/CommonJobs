using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Utilities;

namespace CommonJobs.Domain
{
    public class SlotWithAttachment
    {
        public string SlotId { get; set; }
        public DateTime Date { get; set; }
        public AttachmentReference Attachment { get; set; }

        public static IEnumerable<SlotWithAttachment> GenerateFromNotes(IEnumerable<NoteWithAttachment> notes)
        {
            return notes
                .EmptyIfNull()
                .Where(x => x.Attachment != null)
                .Select(x => new SlotWithAttachment()
                {
                    Attachment = x.Attachment,
                    Date = x.RegisterDate,
                    SlotId = "__NOTE__"
                });
        }

        public static IEnumerable<SlotWithAttachment> GenerateFromImage(ImageAttachment image)
        {
            if (image != null && image.Original != null)
                yield return new SlotWithAttachment()
                {
                    Attachment = image.Original,
                    SlotId = "__PHOTO__"
                };
            ;
            if (image != null && image.Thumbnail != null)
                yield return new SlotWithAttachment()
                {
                    Attachment = image.Thumbnail,
                    SlotId = "__PHOTO_THUMB__"
                };
            yield break;
        }
    }
}
