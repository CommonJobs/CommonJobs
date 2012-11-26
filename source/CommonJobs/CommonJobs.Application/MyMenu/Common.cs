using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.MyMenu
{
    internal static class Common
    {
        public const string DefaultMenuId = "Menu/DefaultMenu";

        public static string GenerateEmployeeMenuId(string userName)
        {
            return "Menu/" + userName;
        }
    }
}
