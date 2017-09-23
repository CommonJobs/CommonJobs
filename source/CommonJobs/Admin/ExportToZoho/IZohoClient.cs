using System;
using System.Threading.Tasks;

namespace Admin.ExportToZoho
{
    public interface IZohoClient : IDisposable
    {
        Task LoginAsync();
        Task LoginIfNeedAsync();
        Task<ZohoResponse> CreateCandidateAsync(Candidate candidate);
    }
}