using CommonJobs.Infrastructure.RavenDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Commands
{
    public class GetEmployeesToEvaluateCommand : Command<List<EmployeeEvaluationDTO>>
    {
        private string _responsible { get; set; }

        public GetEmployeesToEvaluateCommand(string responsible)
        {
            _responsible = responsible;
        }

        public override List<EmployeeEvaluationDTO> ExecuteWithResult()
        {
            return new List<EmployeeEvaluationDTO>();
        }
    }
}
