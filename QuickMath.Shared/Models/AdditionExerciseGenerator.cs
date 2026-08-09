using QuickMath.Shared.Models;

namespace QuickMath.Shared.Services
{
    public class AdditionExerciseGenerator : IExerciseGenerator
    {
        private readonly Random _random = new();

        private const int MinimumNumberA = 1;
        private const int MaximumNumberA = 20;
        private const int MinimumNumberB = 1;
        private const int MaximumNumberB = 20;

        public Exercise NextAddition()
        {
            var a = _random.Next(MinimumNumberA, MaximumNumberA + 1);
            var b = _random.Next(MinimumNumberB, MaximumNumberB + 1);
            return new Exercise { NumberA = a, NumberB = b, CorrectAnswer = a + b };
        }
    }
}