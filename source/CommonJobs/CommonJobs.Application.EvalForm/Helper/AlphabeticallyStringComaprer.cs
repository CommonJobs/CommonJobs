using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Helper
{
    public class AlphabeticallyStringComaprer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x[0] == y[0])
            {
                return x.Length.CompareTo(y.Length);
            }
            else return x[0].CompareTo(y[0]);
        }
    }
}
