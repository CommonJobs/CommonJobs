using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.Configuration;

namespace CommonJobs.Raven.Mvc
{
    public class CommonJobsAuthorizeAttribute : AuthorizeAttribute
    {
        public static Func<string, string> RoleMapToOurs { get; set; }
        public static Func<string, string> RoleMapToAD { get; set; }
        public static Func<CommonJobsAuthorizeAttribute, HttpContextBase, Func<HttpContextBase, bool>, bool> OverrideAuthorize { get; set; }

        private string ApplyMapping(string from, Func<string, string> mapping)
        {
            return from == null ? null
                : string.Join(", ", StringToList(from).Select(x => mapping(x)));
        }

        private static IEnumerable<string> StringToList(string from)
        {
            return from.Split(new[] { ',' }).Select(x => x.Trim());
        }

        public static void SetPrefixMapping(string prefix)
        {
            RoleMapToOurs = x => x.StartsWith(prefix) ? x.Substring(prefix.Length) : x;
            RoleMapToAD = x => prefix + x;
        }

        public static void SetFakeRolesFromString(string actualRoles)
        {
            OverrideAuthorize = (me, httpContext, authorizeCore) =>
            {
                return ValidateRoles(me.Roles, () => actualRoles);
            };
        }

        public static void SetFakeRolesFromSetting(string appSettingsKey)
        {
            OverrideAuthorize = (me, httpContext, authorizeCore) =>
            {
                return ValidateRoles(me.Roles, () => ConfigurationManager.AppSettings[appSettingsKey]);
            };
        }

        private static bool ValidateRoles(string required, Func<string> getActualRoles)
        {
            var actualRoles = getActualRoles();

            if (String.IsNullOrEmpty(actualRoles))
                return false;

            if (String.IsNullOrEmpty(required))
                return true;

            return new HashSet<string>(StringToList(actualRoles)).Intersect(StringToList(required)).Any();
        }

        public new string Roles
        {
            get
            {
                return RoleMapToOurs == null ? base.Roles
                    : ApplyMapping(base.Roles, RoleMapToOurs);
            }
            set
            {
                base.Roles = RoleMapToAD == null ? value
                    : ApplyMapping(value, RoleMapToAD);
            }
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            return OverrideAuthorize == null ? base.AuthorizeCore(httpContext)
                : OverrideAuthorize(this, httpContext, base.AuthorizeCore);
        }
    }
}
