namespace AdaptiveFEM.Models.Materials
{
    public sealed class PMC : Material
    {
        public override MaterialType MaterialType { get; init; }

        public override double RelativePermittivity { get; init; }

        public override double RelativePermeability { get; init; }

        public PMC() : base("Perfect Magnetic Conductor",
            "Ideal magnetic conductor")
        {
            MaterialType = MaterialType.PerfectMagneticConductor;
            RelativePermittivity = 1;
            RelativePermeability = 0;
        }
    }
}
