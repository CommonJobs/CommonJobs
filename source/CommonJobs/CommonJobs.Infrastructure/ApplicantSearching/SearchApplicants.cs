﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Linq;
using CommonJobs.Raven.Infrastructure;
using CommonJobs.Domain;
using CommonJobs.ContentExtraction;
using CommonJobs.Infrastructure.Indexes;

namespace CommonJobs.Infrastructure.ApplicantSearching
{
    public class SearchApplicants : Query<ApplicantSearchResult[]>
    {
        //TODO: add pagination support
        ApplicantSearchParameters Parameters { get; set; }
        
        public SearchApplicants(ApplicantSearchParameters parameters)
        {
            Parameters = parameters;
        }

        public override ApplicantSearchResult[] Execute()
        {
            var query = RavenSession
                .Query<Applicant_QuickSearch.Projection, Applicant_QuickSearch>()
                .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite());

            query = query.Where(x => x.IsApplicant);

            //TODO: enhance it
            if (Parameters.SearchInAttachments)
                query = query.Where(x =>
                    x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Companies.Any(y => y.StartsWith(Parameters.Term))
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term))
                    || x.AttachmentContent.Any(y => y.StartsWith(Parameters.Term)));
            else
                query = query.Where(x =>
                    x.FullName1.StartsWith(Parameters.Term)
                    || x.FullName2.StartsWith(Parameters.Term)
                    || x.Companies.Any(y => y.StartsWith(Parameters.Term))
                    || x.Skills.StartsWith(Parameters.Term)
                    || x.AttachmentNames.Any(y => y.StartsWith(Parameters.Term)));

            if (Parameters.HaveInterview)
                query = query.Where(x => x.HaveInterview);

            if (Parameters.HaveTechnicalInterview)
                query = query.Where(x => x.HaveTechnicalInterview);

            if (Parameters.Highlighted)
                query = query.Where(x => x.IsHighlighted);

            query = query.OrderBy(x => x.FullName1);

            var result = query.AsProjection<ApplicantSearchResult>().ToArray();
            return result;
        }
    }
}
