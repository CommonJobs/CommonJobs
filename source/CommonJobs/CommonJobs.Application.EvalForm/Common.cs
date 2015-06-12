namespace CommonJobs.Application.Evaluations
{
    internal static class Common
    {
        public const string DefaultMenuId = "Evaluations/Default";

        public static string GenerateEvaluationId(string userName)
        {
            return "Evaluations/" + userName;
        }
    }
}
