using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonJobs.Application.EvalForm.Dtos
{
    public class EvaluatorsUpdateDto
    {
        public string UserName { get; set; }

        public EvaluatorAction Action { get; set; }
    }

    public enum EvaluatorAction
    {
        Add,
        Remove
    }
}
