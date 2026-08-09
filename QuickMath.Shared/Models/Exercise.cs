namespace QuickMath.Shared.Models
{
    public class Exercise
    {
        public int NumberA { get; set; }
        public int NumberB { get; set; }

        public string operatorSymbol { get; set; } = "+";
        public int CorrectAnswer { get; set; }

        public string QuestionText => $"{NumberA} {operatorSymbol} {NumberB} = ?";
    }
}