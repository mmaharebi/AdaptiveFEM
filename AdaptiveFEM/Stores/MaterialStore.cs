using AdaptiveFEM.Exceptions;
using AdaptiveFEM.Models.Materials;
using AdaptiveFEM.Models.Materials.Dielectrics;
using System.Collections.Generic;
using System.Linq;

namespace AdaptiveFEM.Stores
{
    public class MaterialStore
    {
        private List<Material> _standardMaterials;

        public List<Material> StandardMaterials
        {
            get => new List<Material>(_standardMaterials);
            private set => _standardMaterials = value;
        }

        private List<Material> _userDefinedMaterials;

        public List<Material> UserDefinedMaterials
        {
            get => new List<Material>(_userDefinedMaterials);
            private set => _userDefinedMaterials = value;
        }

        public List<Material> AllMaterials =>
            new List<Material>(_standardMaterials.Concat(_userDefinedMaterials));

        public MaterialStore()
        {
            _standardMaterials = new List<Material>();
            _userDefinedMaterials = new List<Material>();

            InitializeStandardMaterials();
        }

        private void InitializeStandardMaterials()
        {
            _standardMaterials.Add(new PEC());
            _standardMaterials.Add(new PMC());

            _standardMaterials.Add(new Air());
            _standardMaterials.Add(new Glass());
        }

        private void AddUserDefinedMaterial(Material material)
        {
            foreach (Material existingMaterial in AllMaterials)
            {
                if (existingMaterial.Conflicts(material))
                {
                    throw new MaterialConflictException(existingMaterial, material);
                }
            }

            _userDefinedMaterials.Add(material);
        }
    }
}
