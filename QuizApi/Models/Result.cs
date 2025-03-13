namespace QuizApi.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int StudentId { get; set; }
        public int Score { get; set; }
        public int Total { get; set; }
        public List<QuestionResult> Questions { get; set; } = new List<QuestionResult>();
    }

    public class QuestionResult
    {
        public int QuestionId { get; set; }
        public int SelectedOptionIndex { get; set; }
        public bool IsCorrect { get; set; }
    }
}