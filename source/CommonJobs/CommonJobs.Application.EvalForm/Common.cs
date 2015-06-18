namespace CommonJobs.Application.Evaluations
{
    internal static class Common
    {
        public const string DefaultEvaluationId = "Evaluations/Default";

        public const string DefaultTemplateId = "Template/Default";

        public static string GenerateEvaluationId(string userName)
        {
            return "Evaluations/" + userName;
        }
    }
}
