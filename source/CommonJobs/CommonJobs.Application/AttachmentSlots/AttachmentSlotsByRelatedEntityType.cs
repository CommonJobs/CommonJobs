using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonJobs.Domain;
using CommonJobs.Raven.Infrastructure;
using NLog;
using Raven.Client.Linq;
using CommonJobs.Utilities;

namespace CommonJobs.Application.AttachmentSlots
{
    public class AttachmentSlotsQuery<T> : Query<AttachmentSlot[]>
    {
        private const int MaxSlotsByType = 128;

        private static Logger log = LogManager.GetCurrentClassLogger();

        /* NOTE:
             * Esto no funcionaba:
             *      .Where(x => x.RelatedEntityType == typeof(Employee))
             * porque Newtonsoft Json serializa el tipo con el nombre largo ("CommonJobs.Domain.Employee, CommonJobs.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") 
             * y RavenDB busca por el corto (CommonJobs.Domain.Employee)
             * 
             * Otra opción sería buscar por prefijo del id (http://mattwarren.org/2012/07/12/fun-with-ravendb-documents-keys/)
             * */
        public override AttachmentSlot[] Execute()
        {
            RavenQueryStatistics stats;

            var results = RavenSession.Query<AttachmentSlot>()
                .Statistics(out stats)
                .Take(MaxSlotsByType)
                .Where(x => x.RelatedEntityTypeName == typeof(T).Name)
                .ToArray();

            if (stats.TotalResults != results.Length)
            {
                var message = string.Format("There are too many slots ({0}) for {1} entities.", stats.TotalResults, typeof(T).Name);
                log.Dump(LogLevel.Error, new { RelatedType = typeof(T), MaxSlotsByType, stats, results}, message);
                throw new ApplicationException(message);
            }

            return results;
        }
    }
}
