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

        public bool IsNotDomain { get; }

        public bool IsPhiAssignmentAllowed => IsNotDomain &&
            ((SelectedBoundaryType == BoundaryType.PerfectElectricConductor) ||
            (SelectedMaterial.Name == "Perfect Electric Conductor"));

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
                OnPropertyChanged(nameof(IsPhiAssignmentAllowed));
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

        private double _phi;

        public double Phi
        {
            get => _phi;
            set
            {
                _phi = value;
                OnPropertyChanged(nameof(Phi));
            }
        }
        
        #region Material
        private Material _selectedMaterial;

        public Material SelectedMaterial
        {
            get => _selectedMaterial;
            set
            {
                _selectedMaterial = value;
                OnPropertyChanged(nameof(SelectedMaterial));
                OnPropertyChanged(nameof(IsPhiAssignmentAllowed));
            }
        }

        public List<Material> Materials => _materialStore.AllMaterials;
        #endregion

        public ICommand MakeComponent { get; }

        public ICommand CloseWindow { get; }

        public event EventHandler? CloseThis;

        private readonly MaterialStore _materialStore;

        public ProfileContainerVM(Design design,
            MaterialStore materialStore,
            MessageService messageService,
            ComponentProfileVMBase componentProfileVM,
            ComponentType componentType)
        {
            // Assign fields
            _phi = 0;
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

            // Boundary type combo-box and Phi text-box visibility
            IsNotDomain = !(componentType == ComponentType.Domain);
            if (!IsNotDomain)
            {
                SelectedBoundaryType = BoundaryType.PerfectElectricConductor;
                Phi = 0;
            }
        }

        private void OnCloseThis()
        {
            CloseThis?.Invoke(this, EventArgs.Empty);
        }
    }
}
