using CommonJobs.Application.Evaluations.EmployeeSearching;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class ChangeResponsibleCommand : Command
    {

        public string _evaluatedUserName { get; set; }

        public string _period { get; set; }

        public string _newResponsibleName { get; set; }

        public ChangeResponsibleCommand(string evaluatedUserName, string period, string newResponsibleName)
        {
            _evaluatedUserName = evaluatedUserName;
            _period = period;
            _newResponsibleName = newResponsibleName;
        }

        public override void Execute()
        {
            var evaluationId = EmployeeEvaluation.GenerateEvaluationId(_period, _evaluatedUserName);
            var evaluation = RavenSession.Load<EmployeeEvaluation>(evaluationId);

            if (evaluation == null)
            {
                throw new ApplicationException(string.Format("Error: Evaluación inexistente: {0}.", evaluationId));
            }

            // Check that the new responsible exist
            var newResponsible = RavenSession
                 .Query<Employee_Search.Projection, Employee_Search>()
                 .Where(x => x.IsActive && x.UserName == _newResponsibleName).FirstOrDefault();


            if (newResponsible == null)
            {
                throw new ApplicationException(string.Format("Error: Empleado inexistente: {0}.", _newResponsibleName));
            }

            // Check that new responsible is not the evaluated employee
            if (evaluation.UserName == newResponsible.UserName )
            {
                throw new ApplicationException(string.Format("Error: {0} no puede ser responsable de su propia evaluación.", _newResponsibleName));
            }

                // If the new responsible is the same as the old one, cancel the operation.
                if (evaluation.ResponsibleId == newResponsible.UserName)
            {
                throw new ApplicationException(string.Format("Error: {0} es actualmente el responsable de esta evaluación.", newResponsible.UserName));
            }

            // The only moment when a user cant change an evaluation responsible,
            // is when the evaluation is complete and closed
            if (evaluation.Finished)
            {
                throw new ApplicationException(string.Format("Error: Esta evaluación ya está cerrada. No se puede modificar el responsable."));
            }

            // Load evaluation califications
            var evaluationCalifications = RavenSession.Advanced.LoadStartingWith<EvaluationCalification>(evaluationId + "/").ToList();

            var oldResponsibleCalification = evaluationCalifications.Where(x => x.Owner == CalificationType.Responsible).FirstOrDefault();

            // Change the old responsible califications to an "Evaluator" calification type
            oldResponsibleCalification.Owner = CalificationType.Evaluator;

            // If the new responsible is an evaluator, make its calification as "Responsible" type
            var newResponsibleCalification = evaluationCalifications
                .Where(x => x.Owner == CalificationType.Evaluator && x.EvaluatorEmployee == newResponsible.UserName)
                .FirstOrDefault();

            if (newResponsibleCalification != null)
            {
                newResponsibleCalification.Owner = CalificationType.Responsible;
            }
            // if not, create a new "Responsible" calification
            else
            {
                var newCalification = new EvaluationCalification
                {
                    Owner = CalificationType.Responsible,
                    Period = evaluation.Period,
                    EvaluationId = evaluation.Id,
                    EvaluatedEmployee = evaluation.UserName,
                    EvaluatorEmployee = newResponsible.UserName,
                    TemplateId = "Template/Default"
                };
                RavenSession.Store(newCalification);
            }

            // Update responsible in the evaluation
            evaluation.ResponsibleId = newResponsible.UserName;

            RavenSession.SaveChanges();
        }
    }
}
