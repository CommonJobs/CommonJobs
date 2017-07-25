using System;
using System.Threading.Tasks;

namespace Admin.ExportToZoho
{
    public interface IZohoClient : IDisposable
    {
        Task LoginAsync();
    }
}