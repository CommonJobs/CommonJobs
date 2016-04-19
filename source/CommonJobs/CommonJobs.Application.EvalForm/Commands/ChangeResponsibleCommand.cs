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

        public string _evaluatedUser { get; set; }

        public string _period { get; set; }

        public string _newResponsible { get; set; }

        public ChangeResponsibleCommand (string evaluatedUser, string period, string  newResponsible)
        {
            _evaluatedUser = evaluatedUser;
            _period = period;
            _newResponsible = newResponsible;
        }

        public override void Execute()
        {
            var evaluationId = EmployeeEvaluation.GenerateEvaluationId(_period, _evaluatedUser);
            var evaluation = RavenSession.Load<EmployeeEvaluation>(evaluationId);

            if (evaluation == null)
            {
                throw new ApplicationException(string.Format("Error: Evaluación inexistente: {0}.", evaluationId));
            }

            // Check that the new responsable exist
            var responsible = RavenSession
                 .Query<Employee_Search.Projection, Employee_Search>()
                 .Where(x => x.IsActive && x.UserName == _newResponsible).FirstOrDefault();

            if (responsible == null)
            {
                throw new ApplicationException(string.Format("Error: Empleado inexistente: {0}.", _newResponsible));
            }

            // The only moment when a user cant change an evaluation responsible, is when the evaluation is complete and closed
            if (evaluation.Finished)
            {
                throw new ApplicationException(string.Format("Error: This evaluation is closed. Responsible can't be changed."));
            }

            // Load the responsable calification
            var responsibleCalification = RavenSession.Advanced.LoadStartingWith<EvaluationCalification>(evaluationId + "/").Where(x=>x.Owner==CalificationType.Responsible).FirstOrDefault();

            // If the old responsible has already made a calification,
            // change it to an evaluator calification and create a new responsible one
            if (responsibleCalification.Califications!=null)
            {
                responsibleCalification.Owner = CalificationType.Evaluator;
                var newCalification = new EvaluationCalification
                {
                    Owner = CalificationType.Responsible,
                    Period = evaluation.Period,
                    EvaluationId = evaluation.Id,
                    EvaluatedEmployee = evaluation.UserName,
                    EvaluatorEmployee = responsible.UserName,
                    TemplateId = "Template/Default"
                };
                RavenSession.Store(newCalification);
            }
            else
            {
                responsibleCalification.EvaluatorEmployee = responsible.UserName;
            }

            // Update responsable in the evaluation
            evaluation.ResponsibleId = responsible.Id;

            RavenSession.SaveChanges();
        }
    }
}
