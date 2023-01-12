using AdaptiveFEM.Models.Materials;
using AdaptiveFEM.Services;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public class Domain : Component
    {
        public Domain(Geometry geometry,
            BoundaryType boundaryType,
            Material material,
            MessageService messageService) : base(geometry,
                boundaryType,
                material,
                messageService)
        { }
    }
}
