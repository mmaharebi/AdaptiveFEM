using AdaptiveFEM.Models.Materials;
using AdaptiveFEM.Services;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class Region : Component
    {

        public override Brush Stroke => Brushes.Black;

        public override double StrokeThickness => 1;

        public override Brush Fill => Brushes.LightSeaGreen;

        public Region(Geometry geometry,
            BoundaryType boundaryType,
            double phi,
            Material material,
            MessageService messageService) : base(geometry,
                boundaryType,
                phi,
                material,
                messageService)
        {
        }


        public bool Conflicts(Component component)
        {
            if (component is Domain domain)
            {
                Geometry.Transform = domain.Geometry.Transform;
                CombinedGeometry combinedGeometry =
                    new CombinedGeometry(GeometryCombineMode.Intersect,
                    Geometry, domain.Geometry);
                return !domain.Geometry.FillContains(combinedGeometry);
            }
            else if (component is Region region)
            {
                CombinedGeometry combinedGeometry =
                    new CombinedGeometry(GeometryCombineMode.Intersect,
                    Geometry, region.Geometry);
                if (Geometry.FillContains(region.Geometry) &&
                    region.Geometry.GetArea() <= Geometry.GetArea())
                    return false;
                else
                    return combinedGeometry.GetArea() > 0;
            }
            else
            {
                return true;
            }
        }
    }
}
