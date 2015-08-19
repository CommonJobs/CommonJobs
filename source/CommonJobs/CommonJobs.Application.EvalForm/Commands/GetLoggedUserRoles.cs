using CommonJobs.Domain;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetLoggedUserRoles : Command<string[]>
    {
        private string _loggedUser { get; set; }

        public GetLoggedUserRoles(string loggedUser)
        {
            _loggedUser = loggedUser;
        }

        public override string[] ExecuteWithResult()
        {
            //var user = RavenSession.Query<CommonJobs.Domain.User>().Where(u => u.UserName == _loggedUser).FirstOrDefault();
            var user = RavenSession.Load<User>("Users/" + _loggedUser);
            return user == null || user.Roles == null ? new string[0] : user.Roles;
        }
    }
}
