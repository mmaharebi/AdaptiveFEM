using AdaptiveFEM.Models;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels.ComponentProfiles
{
    public abstract class ComponentProfileVMBase : ViewModelBase
    {
        public abstract ShapeType ShapeType { get; }

        public abstract bool IsGeometryValid { get; }

        public abstract Geometry Geometry { get; }
    }
}
