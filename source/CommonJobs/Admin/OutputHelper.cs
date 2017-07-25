using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin
{
    public interface IOutputHelper
    {
        void WriteLine(string value);
        void DumpObject(object value);
    }

    public class ConsoleOutputHelper : IOutputHelper
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public void DumpObject(object value)
        {
            WriteLine(JsonConvert.SerializeObject(value, Formatting.Indented));
        }
    }
}
