using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveFEM.Models
{
    public class GeometryComponent : Component
    {
        public override Brush Stroke => Brushes.Blue;

        public override double StrokeThickness => 2;

        public override Brush Fill => Brushes.Transparent;

        public GeometryComponent(string name, Geometry geometry)
            : base(name, geometry) { }

    }
}
