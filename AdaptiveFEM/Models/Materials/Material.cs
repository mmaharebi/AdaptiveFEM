using System;

namespace AdaptiveFEM.Models.Materials
{
    public abstract class Material
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public abstract MaterialType MaterialType { get; init; }

        public abstract double RelativePermittivity { get; init; }

        public abstract double RelativePermeability { get; init; }

        public Material(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public bool Conflicts(Material material)
        {
            return this == material;
        }

        public override bool Equals(object? obj)
        {
            return obj is Material material &&
                Name == material.Name;
        }

        public static bool operator ==(Material material1, Material material2)
        {
            return material1.Equals(material2);
        }

        public static bool operator !=(Material material1, Material material2)
        {
            return !material1.Equals(material2);
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name,
                MaterialType,
                RelativePermittivity,
                RelativePermeability);
        }
    }
}
