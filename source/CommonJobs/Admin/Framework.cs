using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin
{
    public interface ICliWorker<T>
    {
        Task Run(T options);
    }

    public interface ICliOptions
    {
        [Option("pause", Required = false, HelpText = "Pause at the end of processing", Default = false)]
        bool Pause { get; set; }
    }

    public class BaseCliOptions : ICliOptions
    {
        public bool Pause { get; set; } = false;
    }
}
