using CommonJobs.Application.EvalForm.DTOs;
using CommonJobs.Domain.Evaluations;
using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class UpdateCalificationsCommand : Command
    {
        private UpdateEvaluationDTO _updateEvaluation;

        public UpdateCalificationsCommand(UpdateEvaluationDTO updateEvaluation)
        {
            _updateEvaluation = updateEvaluation;
        }

        public override void Execute()
        {
            //TODO: Security -> Check the logged user, is he able to perform this action?

            bool updateEvaluationComments = false;
            bool updateEvaluationProject = false;

            var storedEvaluation = RavenSession.Load<EmployeeEvaluation>(_updateEvaluation.EvaluationId);
            if (storedEvaluation == null)
            {
                throw new ApplicationException(string.Format("Error: Evaluación {0} inexistente", _updateEvaluation.EvaluationId));
            }

            foreach (var calification in _updateEvaluation.Califications)
            {
                var storedCalification = RavenSession.Load<EvaluationCalification>(calification.CalificationId);
                if (storedCalification == null)
                {
                    throw new ApplicationException(string.Format("Error: Calificación {0} inexistente para evaluación {1}", calification.CalificationId, _updateEvaluation.EvaluationId));
                }

                // Update the calification comments and values
                storedCalification.Comments = calification.Comments;
                storedCalification.Califications = calification.Items;

                // Update the EvaluationCalification document in the collection (DB)
                RavenSession.Store(storedCalification);

                // If it's the responsible's calification and it's finished, then the company calification should be created
                if (storedCalification.Owner == CalificationType.Responsible)
                {
                    updateEvaluationProject = true;

                    if (storedCalification.Finished)
                    {
                        // Creates the company calification once the responsible one is finished. The evaluator still is the
                        ExecuteCommand(new GenerateCalificationCommand(
                            storedCalification.Period,
                            storedCalification.EvaluatedEmployee,
                            // The EvaluationCalifications document Id will be Evaluations/2015-06/evaluatedUsername/company in this case (Evaluations/2015-06/evaluatedUsername/evaluatorUsername in the other cases)
                            // The only problem is if someday an employee is called C. Ompany (highly unlikely, but who knows)
                            "company",
                            storedCalification.TemplateId,
                            CalificationType.Company,
                            storedCalification.EvaluationId));
                    }
                }

                // If it's the company's calification (or the devolution), then the evaluation project should be updated [ TODO: Improvement - Check if the project has changed before updating it ]
                if (storedCalification.Owner == CalificationType.Company)
                {
                    updateEvaluationComments = true;
                    updateEvaluationProject = true;
                }
            }

            // The evaluation gets updated only if the calification being updated is the company or the final one (devolution)
            if (updateEvaluationComments)
            {
                // Update the evaluation comments
                storedEvaluation.StrengthsComment = _updateEvaluation.Strengths;
                storedEvaluation.ToImproveComment = _updateEvaluation.ToImprove;
                storedEvaluation.ActionPlanComment = _updateEvaluation.ActionPlan;

                if (updateEvaluationProject)
                {
                    // Update the evaluation project
                    storedEvaluation.Project = _updateEvaluation.Project;
                }

                // Update the EmployeeEvaluation document in the collection (DB)
                RavenSession.Store(storedEvaluation);
            }
        }
    }
}
