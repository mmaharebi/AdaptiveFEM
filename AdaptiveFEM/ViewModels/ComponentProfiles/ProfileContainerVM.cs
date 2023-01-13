using AdaptiveFEM.Models;
using AdaptiveFEM.Stores;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using AdaptiveFEM.Models.Materials;
using System.Linq;
using AdaptiveFEM.Commands.ComponentCommands;
using AdaptiveFEM.Commands;
using AdaptiveFEM.Services;

namespace AdaptiveFEM.ViewModels.ComponentProfiles
{
    public class ProfileContainerVM : ViewModelBase
    {
        private ComponentProfileVMBase _componentProfileVM;

        public ComponentProfileVMBase ComponentProfileVM
        {
            get => _componentProfileVM;
            set
            {
                _componentProfileVM = value;
                OnPropertyChanged(nameof(ComponentProfileVM));
            }
        }

        public bool IsGeometryValid => _componentProfileVM.IsGeometryValid;

        public ComponentType ComponentType { get; init; }

        public ShapeType ShapeType => ComponentProfileVM.ShapeType;

        public string WindowTitle => $"{ComponentType}: {ShapeType}";

        #region Boundary type
        private BoundaryType _selectedBoundaryType;

        public BoundaryType SelectedBoundaryType
        {
            get => _selectedBoundaryType;
            set
            {
                _selectedBoundaryType = value;
                OnPropertyChanged(nameof(SelectedBoundaryType));
            }
        }

        public List<BoundaryType> BoundaryTypes
        {
            get
            {
                List<BoundaryType> boundaryTypes = new List<BoundaryType>();
                foreach (BoundaryType boundaryType in Enum.GetValues(typeof(BoundaryType)))
                {
                    boundaryTypes.Add(boundaryType);
                }

                return boundaryTypes;
            }
        }
        #endregion

        #region Material
        private Material _selectedMaterial;

        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                _selectedMaterial = value;
                OnPropertyChanged(nameof(SelectedMaterial));
            }
        }

        public List<Material> Materials => _materialStore.AllMaterials;
        #endregion

        public ICommand MakeComponent { get; }

        public ICommand CloseWindow { get; }

        public event EventHandler? CloseThis;

        private readonly Design _design;
        private readonly MaterialStore _materialStore;

        public ProfileContainerVM(Design design,
            MaterialStore materialStore,
            MessageService messageService,
            ComponentProfileVMBase componentProfileVM,
            ComponentType componentType)
        {
            // Assign fields
            _design = design;
            _materialStore = materialStore;
            _componentProfileVM = componentProfileVM;
            ComponentType = componentType;

            // Instantiate fields
            _selectedBoundaryType = BoundaryType.Dielectric;
            _selectedMaterial =
                materialStore.AllMaterials.Where(m => m.Name == "Air").First();

            // Instantiate commands
            MakeComponent = new MakeComponent(design, messageService, this, OnCloseThis);
            CloseWindow = new CloseWindow(OnCloseThis);
        }

        private void OnCloseThis()
        {
            CloseThis?.Invoke(this, EventArgs.Empty);
        }
    }
}
