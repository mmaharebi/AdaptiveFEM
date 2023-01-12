namespace AdaptiveFEM.Models.Materials
{
    public sealed class PEC : Material
    {
        public override MaterialType MaterialType { get; init; }
        
        public override double RelativePermittivity { get; init; }
        
        public override double RelativePermeability { get; init; }

        public PEC() : base("Perfect Electric Conductor", "Ideal electric conductor")
        {
            MaterialType = MaterialType.PerfectElectricConductor;
            RelativePermittivity = 0;
            RelativePermeability = 1;
        }

    }
}
