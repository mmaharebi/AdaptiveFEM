using AdaptiveFEM.Commands;
using AdaptiveFEM.Models;
using System.Windows.Input;

namespace AdaptiveFEM.ViewModels
{
    public class TabSolutionVM : ViewModelBase
    {
        public ICommand Solve { get; }

        public TabSolutionVM(Design design)
        {
            Solve = new Solve(design);
        }
    }
}
