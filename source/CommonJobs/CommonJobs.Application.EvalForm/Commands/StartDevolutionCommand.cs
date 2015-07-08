using CommonJobs.Application.Evaluations.EmployeeSearching;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class StartDevolutionCommand : Command
    {
        private string _evaluationId;

        private string _loggedUser;

        public StartDevolutionCommand(string evaluationId, string loggedUser)
        {
            _evaluationId = evaluationId;

            _loggedUser = loggedUser;
        }

        public override void Execute()
        {
            var evaluation = RavenSession.Load<EmployeeEvaluation>(_evaluationId);

            if (_loggedUser != evaluation.ResponsibleId)
            {
                throw new ApplicationException(string.Format("Error: Solo el responsable de la evaluación ({0}) puede iniciar su devolución", evaluation.ResponsibleId));
            }

            Employee_Search.Projection employee = RavenSession
            .Query<Employee_Search.Projection, Employee_Search>()
            .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
            .Where(x => x.IsActive && x.UserName == evaluation.UserName).FirstOrDefault();

            evaluation.ReadyForDevolution = true;
            evaluation.CurrentPosition = employee.CurrentPosition;
            evaluation.Seniority = employee.Seniority;

            RavenSession.Store(evaluation);
        }
    }
}
