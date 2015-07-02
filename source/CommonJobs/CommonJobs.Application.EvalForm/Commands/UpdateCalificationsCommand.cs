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
        private const string COMPANY = "_company";

        private UpdateEvaluationDTO _updateEvaluation;

        private string _loggedUser;

        public UpdateCalificationsCommand(UpdateEvaluationDTO updateEvaluation, string loggedUser)
        {
            _updateEvaluation = updateEvaluation;

            _loggedUser = loggedUser;
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

                // Checks if the user trying to update the califications can actually do it
                if (CanUpdate(_loggedUser, storedEvaluation, storedCalification))
                {
                    UpdateCalification(calification, storedCalification, _updateEvaluation.Finished);

                    // If it's the responsible's or the company's calification, then the evaluation project should be updated [ TODO: Improvement - Check if the project has changed before updating it ]
                    updateEvaluationProject = (storedCalification.Owner == CalificationType.Responsible || storedCalification.Owner == CalificationType.Company);
                    // If it's the company's calification (or devolution), then the evaluation comments should be updated [ TODO: Improvement - Check if the comments have changed before updating it ]
                    updateEvaluationComments = storedCalification.Owner == CalificationType.Company;

                    // If it's the responsible's calification and it's finished, then the company calification should be created
                    CreateCompanyEvaluation(storedCalification);
                }
            }

            // The evaluation gets updated only if the calification being updated is the company or the final one (devolution)
            UpdateEvaluation(updateEvaluationComments, updateEvaluationProject, storedEvaluation);
        }

        private void UpdateCalification(UpdateCalificationDTO calification, EvaluationCalification storedCalification, bool finished)
        {
            // Update the calification comments and values
            storedCalification.Comments = calification.Comments;
            storedCalification.Califications = calification.Items;
            storedCalification.Finished = finished;

            // Update the EvaluationCalification document in the collection (DB)
            RavenSession.Store(storedCalification);
        }

        private void CreateCompanyEvaluation(EvaluationCalification storedCalification)
        {
            if (storedCalification.Owner == CalificationType.Responsible && storedCalification.Finished)
            {
                ExecuteCommand(new GenerateCalificationCommand(storedCalification.Period, storedCalification.EvaluatedEmployee, COMPANY, storedCalification.TemplateId, CalificationType.Company,
                    storedCalification.EvaluationId));
            }
        }

        private void UpdateEvaluation(bool updateEvaluationComments, bool updateEvaluationProject, EmployeeEvaluation storedEvaluation)
        {
            if (updateEvaluationComments)
            {
                storedEvaluation.StrengthsComment = _updateEvaluation.Strengths;
                storedEvaluation.ToImproveComment = _updateEvaluation.ToImprove;
                storedEvaluation.ActionPlanComment = _updateEvaluation.ActionPlan;

                if (updateEvaluationProject)
                {
                    storedEvaluation.Project = _updateEvaluation.Project;
                }

                // Update the EmployeeEvaluation document in the collection (DB)
                RavenSession.Store(storedEvaluation);
            }
        }

        private bool CanUpdate(string loggedUser, EmployeeEvaluation evaluation, EvaluationCalification calification)
        {
            //Auto evaluator
            if (loggedUser == evaluation.UserName)
            {
                if (calification.Owner == CalificationType.Auto && calification.EvaluatorEmployee == loggedUser) return true;
            }

            //Responsible
            if (loggedUser == evaluation.ResponsibleId)
            {
                if (calification.Owner == CalificationType.Responsible && calification.EvaluatorEmployee == loggedUser) return true;

                if (calification.Owner == CalificationType.Auto && evaluation.ReadyForDevolution) return true;

                if (calification.Owner == CalificationType.Company && calification.EvaluatorEmployee == COMPANY) return true;
            }

            //Evaluator
            if (calification.Owner == CalificationType.Evaluator && calification.EvaluatorEmployee == loggedUser) return true;

            return false;
        }
    }
}
