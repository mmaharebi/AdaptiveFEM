using AdaptiveFEM.Commands;
using AdaptiveFEM.Models;
using System.Windows.Input;

namespace AdaptiveFEM.ViewModels
{
    public class TabMeshVM : ViewModelBase
    {
        public ICommand GenerateMesh { get; }

        public TabMeshVM(Design design)
        {
            GenerateMesh = new GenerateMesh(design);
        }

    }
}
