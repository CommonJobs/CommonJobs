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

            evaluation.ReadyForDevolution = true;

            RavenSession.Store(evaluation);
        }
    }
}
