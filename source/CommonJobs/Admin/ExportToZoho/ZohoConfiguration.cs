﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.ExportToZoho
{
    public class ZohoConfiguration
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string LoginUriTemplate { get; set; }
        public string GeneralUriTemplate { get; set; }
    }
}
