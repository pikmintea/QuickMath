using QuickMath.Shared.Models;

namespace QuickMath.Shared.Services
{
    public interface IExerciseGenerator
    {
        Exercise NextAddition();
        Exercise NextSubtraction();

        Exercise NextMultiplication();

        Exercise NextDivision();

        void SetDifficulty(string difficulty);
    }
}