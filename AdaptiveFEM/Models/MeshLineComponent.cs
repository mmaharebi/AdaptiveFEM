using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class MeshLineComponent : Component
    {
        public override Brush Stroke => Brushes.DarkBlue;

        public override double StrokeThickness => 2;

        public override Brush Fill => Brushes.Transparent;

        public MeshLineComponent(Geometry geometry) : base(geometry) { }
    }
}
