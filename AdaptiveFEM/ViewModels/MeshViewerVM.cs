using AdaptiveFEM.Commands.ViewerCommands.MeshViewerCommands;
using AdaptiveFEM.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace AdaptiveFEM.ViewModels
{
    public class MeshViewerVM : ViewerVMBase
    {
        public override ObservableCollection<Component> Items { get; set; }

        public ICommand UpdateMeshItems { get; }

        public MeshViewerVM(Design design) : base(design)
        {
            Items = new ObservableCollection<Component>();

            UpdateMeshItems = new UpdateMeshItems(GetMeshLines);
        }

        private void GetMeshLines()
        {
            int NOcoordinateItems = 3;

            TransformGroup transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform(1, -1));
            transform.Children
                .Add(new TranslateTransform(coordinateCenterPosition.X,
                coordinateCenterPosition.Y));

            for (int i = 0; i < design.Solution.MeshLines.Count; i++)
            {
                design.Solution.MeshLines[i].Geometry.Transform = transform;
                Items.Insert(Items.Count - NOcoordinateItems,
                    design.Solution.MeshLines[i]);
            }
        }
    }
}
