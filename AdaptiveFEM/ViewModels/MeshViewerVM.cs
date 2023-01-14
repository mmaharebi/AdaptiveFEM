using AdaptiveFEM.Models;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AdaptiveFEM.ViewModels
{
    public class MeshViewerVM : ViewerVMBase
    {
        private ObservableCollection<Path> _meshLines;

        public override ObservableCollection<Path> Items
        {
            get => _meshLines;
            set
            {
                _meshLines = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public MeshViewerVM(Design design) : base(design)
        {
            _meshLines = new ObservableCollection<Path>();

            design.DesignChanged += OnDesignChanged;
        }

        private void OnDesignChanged(object? sender, System.EventArgs e)
        {
            UpdateItems();
        }

        protected override void UpdateItems()
        {
            Items.Clear();

            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(1, -1));
            transformGroup.Children.Add(new TranslateTransform(CoordinateCenter.X,
                CoordinateCenter.Y));

            design.Solution.UniformMesh.ForEach(um =>
            {
                Path p = geometryElements.MeshLinePath(um);
                p.RenderTransform = transformGroup;
                Items.Add(p);
            });

            Items.Add(CoordinateCircle);
            Items.Add(XAxis);
            Items.Add(YAxis);

            OnPropertyChanged(nameof(Items));
        }
    }
}
