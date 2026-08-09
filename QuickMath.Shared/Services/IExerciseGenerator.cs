using QuickMath.Shared.Models;

namespace QuickMath.Shared.Services
{
    public interface IExerciseGenerator
    {
        Exercise NextAddition();
        Exercise NextSubtraction();

        void SetDifficulty(string difficulty);
    }
}