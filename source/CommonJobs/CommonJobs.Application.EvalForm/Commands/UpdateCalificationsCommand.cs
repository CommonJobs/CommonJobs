using CommonJobs.Application.EvalForm.Dtos;
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

        private UpdateEvaluationDto _updateEvaluation;

        private string _loggedUser;

        public UpdateCalificationsCommand(UpdateEvaluationDto updateEvaluation, string loggedUser)
        {
            _updateEvaluation = updateEvaluation;

            _loggedUser = loggedUser;
        }

        public override void Execute()
        {
            bool updateEvaluationComments = false;
            bool updateEvaluationProject = false;

            var storedEvaluation = RavenSession.Load<EmployeeEvaluation>(_updateEvaluation.EvaluationId);
            if (storedEvaluation == null)
            {
                throw new ApplicationException(string.Format("Error: Evaluación {0} inexistente", _updateEvaluation.EvaluationId));
            }
            else if (storedEvaluation.Finished)
            {
                throw new ApplicationException(string.Format("Error: Evaluación {0} cerrada", _updateEvaluation.EvaluationId));
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
                    UpdateCalification(calification, storedCalification, _updateEvaluation.CalificationFinished);

                    // If it's the responsible's or the company's calification, then the evaluation project should be updated [ TODO: Improvement - Check if the project has changed before updating it ]
                    updateEvaluationProject |= (storedCalification.Owner == CalificationType.Responsible || storedCalification.Owner == CalificationType.Company);
                    // If it's the company's calification (or devolution), then the evaluation comments should be updated [ TODO: Improvement - Check if the comments have changed before updating it ]
                    updateEvaluationComments |= storedCalification.Owner == CalificationType.Company;

                    // If it's the responsible's calification and it's finished, then the company calification should be created
                    if (storedCalification.Owner == CalificationType.Responsible && storedCalification.Finished)
                    {
                        CreateCompanyEvaluation(storedCalification);
                    }
                }
            }

            if (updateEvaluationProject || updateEvaluationComments || (_loggedUser == storedEvaluation.ResponsibleId && storedEvaluation.ReadyForDevolution))
            {
                storedEvaluation.Project = updateEvaluationProject ? _updateEvaluation.Project : storedEvaluation.Project;
                storedEvaluation.StrengthsComment = updateEvaluationComments ? _updateEvaluation.Strengths : storedEvaluation.StrengthsComment;
                storedEvaluation.ToImproveComment = updateEvaluationComments ? _updateEvaluation.ToImprove : storedEvaluation.ToImproveComment;
                storedEvaluation.ActionPlanComment = updateEvaluationComments ? _updateEvaluation.ActionPlan : storedEvaluation.ActionPlanComment;
                storedEvaluation.Finished = (_loggedUser == storedEvaluation.ResponsibleId && storedEvaluation.ReadyForDevolution) ? _updateEvaluation.EvaluationFinished : storedEvaluation.Finished;
                RavenSession.Store(storedEvaluation);
            }
        }        

        private void UpdateCalification(UpdateCalificationDto calification, EvaluationCalification storedCalification, bool finished)
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
            ExecuteCommand(new GenerateCalificationCommand(storedCalification.Period, storedCalification.EvaluatedEmployee, COMPANY, storedCalification.TemplateId, CalificationType.Company,
                storedCalification.EvaluationId));
        }        

        private bool CanUpdate(string loggedUser, EmployeeEvaluation evaluation, EvaluationCalification calification)
        {            
            //Auto evaluator
            if (loggedUser == evaluation.UserName)
            {
                if (calification.Owner == CalificationType.Auto && calification.EvaluatorEmployee == loggedUser && !calification.Finished) return true;
            }

            //Responsible
            if (loggedUser == evaluation.ResponsibleId)
            {
                if (calification.Owner == CalificationType.Responsible && calification.EvaluatorEmployee == loggedUser && !calification.Finished) return true;

                // Auto calification can be edited (by the responsible) once finished (at the devolution)
                if (calification.Owner == CalificationType.Auto && evaluation.ReadyForDevolution) return true;

                // Company calification can be edited (by the responsible) once finished (at the devolution)
                if (calification.Owner == CalificationType.Company && calification.EvaluatorEmployee == COMPANY) return true;
            }

            //Evaluator
            if (calification.Owner == CalificationType.Evaluator && calification.EvaluatorEmployee == loggedUser && !calification.Finished) return true;

            return false;
        }
    }
}
