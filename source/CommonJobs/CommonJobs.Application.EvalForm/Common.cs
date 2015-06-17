namespace CommonJobs.Application.Evaluations
{
    internal static class Common
    {
        //public const string DefaultEvaluationId = "Evaluations/Default";

        public const string DefaultTemplateId = "Template/Default";

        public static string GenerateEvaluationId(string period, string userName)
        {
            return "Evaluations/" + period + "/" + userName;
        }

        public static string GenerateCalificationId(string period, string userName, string evaluator)
        {
            return "Evaluations/" + period + "/" + userName + "/" + evaluator;
        }
    }
}
