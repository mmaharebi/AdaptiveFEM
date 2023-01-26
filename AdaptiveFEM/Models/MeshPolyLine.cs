using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class MeshPolyLine
    {
        public PointCollection Points { get; set; }

        public SolidColorBrush Stroke { get; set; }

        public double StrokeThickness { get; set; } = 1.0;

        public TransformGroup TransformGroup { get; set; }
    }
}
