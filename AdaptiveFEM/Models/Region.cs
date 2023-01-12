using AdaptiveFEM.Models.Materials;
using AdaptiveFEM.Services;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class Region : Component
    {
        public Region(Geometry geometry,
            BoundaryType boundaryType,
            Material material,
            MessageService messageService) : base(geometry,
                boundaryType,
                material,
                messageService)
        {
        }

        public bool Conflicts(Component component)
        {
            if (component is Domain domain)
            {
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
