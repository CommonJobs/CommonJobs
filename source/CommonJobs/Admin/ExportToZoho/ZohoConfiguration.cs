using System;
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
        public string LoginUriTemplate { get; set; } = "https://accounts.zoho.com/apiauthtoken/nb/create?SCOPE=ZohoRecruit/recruitapi&EMAIL_ID={username}&PASSWORD={password}&DISPLAY_NAME=recruit";
        public string GeneralUriTemplate { get; set; } = "https://recruit.zoho.com/recruit/private/xml/{module}/{method}?authtoken={token}&scope=recruitapi&version=2";
    }
}
