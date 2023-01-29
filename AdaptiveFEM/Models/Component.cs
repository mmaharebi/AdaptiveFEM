using AdaptiveFEM.Models.Materials;
using AdaptiveFEM.Models.Materials.Dielectrics;
using AdaptiveFEM.Services;
using System.Windows.Media;

namespace AdaptiveFEM.Models
{
    public abstract class Component
    {
        public int LayerIndex { get; }

        public string Name { get; set; }

        public Geometry Geometry { get; }

        public abstract Brush Stroke { get; }

        public abstract double StrokeThickness { get; }

        public abstract Brush Fill { get; }

        public BoundaryType BoundaryType { get; }

        public double Phi { get; }

        public Material Material { get; }

        private static int _meshLineCounter;

        protected readonly MessageService messageService;

        // For domain and region
        protected Component(Geometry geometry,
            BoundaryType boundaryType,
            double phi,
            Material material,
            MessageService messageService)
        {
            LayerIndex = ComponentLayerIndexGenerator.NewLayerIndex();
            Name = $"Component-{LayerIndex}";
            Geometry = geometry;
            BoundaryType = boundaryType;
            Phi = phi;
            Material = material;
            this.messageService = messageService;
        }

        // For coordinate elements
        protected Component(string name,
            Geometry geometry)
        {
            LayerIndex = -1;
            Name = $"Coordinate-{name}";
            Geometry = geometry;
            BoundaryType = BoundaryType.Dielectric;
            Phi = double.NaN;
            Material = new Air();
            messageService = new MessageService();
        }

        // For Mesh-lines
        protected Component(Geometry geometry)
        {
            LayerIndex = -2;
            Name = $"Mesh-line-{_meshLineCounter++}";
            Geometry = geometry;
            BoundaryType = BoundaryType.Dielectric;
            Phi = double.NaN;
            Material = new Air();
            messageService = new MessageService();
        }
    }
}
