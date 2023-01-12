namespace AdaptiveFEM.Models.Materials.Dielectrics
{
    public class Dielectric : Material
    {
        public override MaterialType MaterialType { get; init; }

        public override double RelativePermittivity { get; init; }

        public sealed override double RelativePermeability { get; init; }

        public Dielectric(string name,
            string description,
            double relativePermittivity) : base(name, description)
        {
            MaterialType = MaterialType.Dielectric;
            RelativePermittivity = relativePermittivity;
        }
    }
}
