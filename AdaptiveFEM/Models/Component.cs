using AdaptiveFEM.Models.Materials;
using AdaptiveFEM.Services;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public abstract class Component
    {
        public Geometry Geometry { get; }

        public BoundaryType BoundaryType { get; }

        public Material Material { get; }

        protected readonly MessageService messageService;

        protected Component(Geometry geometry,
            BoundaryType boundaryType,
            Material material,
            MessageService messageService)
        {
            this.Geometry = geometry;
            BoundaryType = boundaryType;
            Material = material;
            this.messageService = messageService;
        }
    }
}
