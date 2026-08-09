using QuickMath.Shared.Models;

namespace QuickMath.Shared.Services
{
    public class ExerciseGenerator : IExerciseGenerator
    {
        private readonly Random _random = new();

        public int MinimumNumberA = 1;
        public int MaximumNumberA = 20;
        public int MinimumNumberB = 1;
        public int MaximumNumberB = 20;

        public Exercise NextAddition()
        {
            string operatorSymbol = "+";
            var a = _random.Next(MinimumNumberA, MaximumNumberA + 1);
            var b = _random.Next(MinimumNumberB, MaximumNumberB + 1);
            return new Exercise { NumberA = a, NumberB = b, CorrectAnswer = a + b, operatorSymbol = operatorSymbol };
        }
        public Exercise NextSubtraction()
        {
            string operatorSymbol = "-";
            var a = _random.Next(MinimumNumberA, MaximumNumberA + 1);
            var b = _random.Next(MinimumNumberB, MaximumNumberB + 1);
            return new Exercise { NumberA = a, NumberB = b, CorrectAnswer = a - b, operatorSymbol = operatorSymbol };
        }
        public Exercise NextMultiplication()
        {
            string operatorSymbol = "*";
            var a = _random.Next(MinimumNumberA, MaximumNumberA + 1);
            var b = _random.Next(MinimumNumberB, MaximumNumberB + 1);
            return new Exercise { NumberA = a, NumberB = b, CorrectAnswer = a * b, operatorSymbol = operatorSymbol };
        }
        public Exercise NextDivision()
        {
            string operatorSymbol = "/";
            var a = _random.Next(MinimumNumberA, MaximumNumberA + 1);
            var b = _random.Next(MinimumNumberB, MaximumNumberB + 1);
            return new Exercise { NumberA = a, NumberB = b, CorrectAnswer = a / b, operatorSymbol = operatorSymbol };
        }
        public void SetDifficulty(string difficulty)
        {
            switch (difficulty)
            {
                case "easy++":
                    MinimumNumberA = 1;
                    MaximumNumberA = 10;
                    MinimumNumberB = 1;
                    MaximumNumberB = 10; 
                    break;
                case "easy":
                    MinimumNumberA = 10;
                    MaximumNumberA = 50;
                    MinimumNumberB = 10;
                    MaximumNumberB = 50;
                    break;
                case "medium":
                    MinimumNumberA = 20;
                    MaximumNumberA = 100;
                    MinimumNumberB = 20;
                    MaximumNumberB = 100;
                    break;
                case "hard":
                    MinimumNumberA = 50;
                    MaximumNumberA = 50;
                    MinimumNumberB = 200;
                    MaximumNumberB = 400;
                    break;

                    case "hard++":
                    MinimumNumberA = 100;
                    MaximumNumberA = 100;
                    MinimumNumberB = 800;
                    MaximumNumberB = 800;
                    break;

            }

        }
    }
}