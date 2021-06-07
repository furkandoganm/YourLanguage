namespace YourLanguage.Settings
{
    public class AppSettings
    {
        public static string Title { get; set; }
        public static string AcceptedImageExtensions { get; set; }
        public static bool NewUserActive { get; set; }
        public static double AcceptedImageMaximumLength { get; set; }
        public static int RecordsCountPerPage { get; set; }

        public static int QuizListCount { get; set; }
        public static int MaxWordListCount { get; set; }
        public static int MaxQuestionCount { get; set; }

        public static int TestCount { get; set; }
    }
}
