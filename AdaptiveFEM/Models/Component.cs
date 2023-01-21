using AdaptiveFEM.Models.Materials;
using AdaptiveFEM.Models.Materials.Dielectrics;
using AdaptiveFEM.Services;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public abstract class Component
    {
        public string Name { get; set; }

        public Geometry Geometry { get; }

        public abstract Brush Stroke { get; }

        public abstract double StrokeThickness { get; }

        public abstract Brush Fill { get; }

        public BoundaryType BoundaryType { get; }

        public Material Material { get; }

        private static int _meshLineCounter;

        protected readonly MessageService messageService;

        // For domain and region
        protected Component(Geometry geometry,
            BoundaryType boundaryType,
            Material material,
            MessageService messageService)
        {
            Name = $"Component-{ComponentIdGenerator.NewId()}";
            this.Geometry = geometry;
            BoundaryType = boundaryType;
            Material = material;
            this.messageService = messageService;
        }

        // For coordinate elements
        protected Component(string name,
            Geometry geometry)
        {
            Name = name;
            Geometry = geometry;
            BoundaryType = BoundaryType.Dielectric;
            Material = new Air();
            messageService = new MessageService();
        }

        // For Mesh-lines
        protected Component(Geometry geometry)
        {
            Name = $"Mesh-line-{_meshLineCounter++}";
            Geometry = geometry;
            BoundaryType = BoundaryType.Dielectric;
            Material = new Air();
            messageService = new MessageService();
        }
    }
}
