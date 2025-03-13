namespace QuizApi.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Duration { get; set; } // Duration in minutes
        public List<Question> Questions { get; set; } = new List<Question>();
        public int TeacherId { get; set; } // Teacher who created the test
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
    }
}